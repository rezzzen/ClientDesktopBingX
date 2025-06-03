using MongoDB.Driver;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Windows.Forms;
using System.Globalization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Timers;

namespace ClientDesktopBingX
{
    public partial class Form1 : Form
    {
        private readonly IMongoClient _mongoClient;
        private IMongoCollection<CandleData> _currentCollection;
        private readonly System.Timers.Timer _timer;

        public Form1()
        {
            InitializeComponent();



            plotView1.Controller = new PlotController();
            plotView1.Controller.UnbindAll();

            // Панорамирование: зажать ЛКМ + перемещение мыши
            plotView1.Controller.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);

            // Масштабирование колесом мыши
            plotView1.Controller.BindMouseWheel(PlotCommands.ZoomWheel);




            _mongoClient = new MongoClient("mongodb://localhost:27017");

            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, t => true);

            // Инициализация таймера для обновления времени
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += UpdateTime;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            // Инициализация элементов управления
            CB_TIMEFRAME.Items.AddRange(new[] { "5M", "10M", "30M", "1H", "4H", "1D", "1W" });
            CB_TIMEFRAME.SelectedIndex = 0;

            CB_INDICATORS.Items.AddRange(new[] { "None", "SMA 20", "EMA 20", "RSI 14" });
            CB_INDICATORS.SelectedIndex = 0;

            UD_COUNT_BARS.Minimum = 1;
            UD_COUNT_BARS.Value = 100;

            // Подписка на события
            CB_TIMEFRAME.SelectedIndexChanged += ReloadDataEvent;
            CB_INDICATORS.SelectedIndexChanged += ReloadDataEvent;
            UD_COUNT_BARS.ValueChanged += ReloadDataEvent;
            this.Load += Form1_Load;
            CB_INSTRUMENTS.SelectedIndexChanged += CB_INSTRUMENTS_SelectedIndexChanged;
        }


        private void ReloadDataEvent(object sender, EventArgs e)
        {
            if (CB_INSTRUMENTS.SelectedItem != null)
                LoadDataAndPlot();
        }

        private void UpdateTime(object sender, ElapsedEventArgs e)
        {
            SafeUpdateLabel(L_TIME, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
        }

        private void SafeUpdateLabel(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action(() => label.Text = text));
            }
            else
            {
                label.Text = text;
            }
        }




        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadInstrumentsAsync();
            await LoadDataAndPlot(); // Загрузка данных после инициализации инструментов
        }

        private async Task LoadInstrumentsAsync()
        {
            try
            {
                CB_INSTRUMENTS.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var database = _mongoClient.GetDatabase("CDA");

                // Получаем все коллекции
                var collections = await database.ListCollectionNamesAsync();
                var filteredCollections = (await collections.ToListAsync())
                    .Where(c => !c.StartsWith("system."))
                    .ToList();

                // Создаем список для хранения информации о коллекциях
                var collectionStats = new List<Tuple<string, long>>();

                // Параллельно получаем количество документов
                var tasks = filteredCollections.Select(async collectionName =>
                {
                    var collection = database.GetCollection<BsonDocument>(collectionName);
                    var count = await collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
                    return (Name: collectionName, Count: count);
                });

                var results = await Task.WhenAll(tasks);

                // Сортируем по убыванию количества данных
                var sortedCollections = results
                    .OrderByDescending(x => x.Count)
                    .ToList();

                // Формируем список инструментов
                var instruments = sortedCollections
                    .Select(c => c.Name.Replace("_5M", "").Replace("_", "/"))
                    .ToList();

                SafeUpdateComboBox(instruments);
                _currentCollection = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки инструментов: {ex.Message}");
            }
            finally
            {
                CB_INSTRUMENTS.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void SafeUpdateComboBox(List<string> instruments)
        {
            if (CB_INSTRUMENTS.InvokeRequired)
            {
                CB_INSTRUMENTS.Invoke(new Action(() =>
                {
                    CB_INSTRUMENTS.DataSource = instruments;
                    if (instruments.Any()) CB_INSTRUMENTS.SelectedIndex = 0;
                }));
            }
            else
            {
                CB_INSTRUMENTS.DataSource = instruments;
                if (instruments.Any()) CB_INSTRUMENTS.SelectedIndex = 0;
            }
        }

        private async Task LoadDataAndPlot()
        {
            if (_currentCollection == null) return;

            try
            {
                plotView1.Model = null;
                Cursor = Cursors.WaitCursor;

                var count = (int)UD_COUNT_BARS.Value;
                var timeframe = CB_TIMEFRAME.SelectedItem.ToString();

                // Загрузка всех 5M баров
                var sortDefinition = Builders<CandleData>.Sort
                    .Descending(d => d.Date)
                    .Descending(d => d.Time);

                var fiveMinuteCandles = await _currentCollection.Find(_ => true)
                    .Sort(sortDefinition)
                    .Limit(1000) // Берем больше данных для агрегации
                    .ToListAsync();

                fiveMinuteCandles = fiveMinuteCandles.OrderBy(c => c.Timestamp).ToList();

                // Агрегация в выбранный таймфрейм
                var aggregatedCandles = AggregateCandles(fiveMinuteCandles, timeframe)
                    .TakeLast(count) // Берем последние N баров
                    .ToList();

                // Создаем новую модель графика
                var plotModel = new PlotModel
                {
                    Title = $"{CB_INSTRUMENTS.SelectedItem} [{CB_TIMEFRAME.SelectedItem}]",
                    TextColor = OxyColors.Gray,
                    PlotAreaBorderColor = OxyColors.Gray,
                    IsLegendVisible = true, // Показать легенду
                    
                };

                // Добавляем оси
                var dateAxis = new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    StringFormat = "HH:mm",
                    Title = "Time",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot
                };

                var valueAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Price",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot
                };

                plotModel.Axes.Add(dateAxis);
                plotModel.Axes.Add(valueAxis);

                // Добавляем свечной график
                var candleSeries = new CandleStickSeries
                {
                    Title = "Candles",
                    Color = OxyColors.DarkGray,
                    IncreasingColor = OxyColors.Green,
                    DecreasingColor = OxyColors.Red
                };

                foreach (var candle in aggregatedCandles)
                {
                    candleSeries.Items.Add(new HighLowItem(
                        DateTimeAxis.ToDouble(candle.Timestamp),
                        candle.High,
                        candle.Low,
                        candle.Open,
                        candle.Close));
                }

                plotModel.Series.Add(candleSeries);

                // Добавляем индикаторы
                var selectedIndicator = CB_INDICATORS.SelectedItem?.ToString();
                if (selectedIndicator != null && selectedIndicator != "None")
                {
                    AddIndicator(plotModel, aggregatedCandles, selectedIndicator);
                }

                // Обновляем рекомендации
                UpdateRecommendation(aggregatedCandles);

                // Привязываем модель к элементу управления
                plotView1.Model = plotModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }



        private List<CandleData> AggregateCandles(List<CandleData> sourceCandles, string timeframe)
        {
            var aggregated = new List<CandleData>();
            TimeSpan interval;

            switch (timeframe)
            {
                case "5M": interval = TimeSpan.FromMinutes(5); break;
                case "10M": interval = TimeSpan.FromMinutes(10); break;
                case "30M": interval = TimeSpan.FromMinutes(30); break;
                case "1H": interval = TimeSpan.FromHours(1); break;
                case "4H": interval = TimeSpan.FromHours(4); break;
                case "1D": interval = TimeSpan.FromDays(1); break;
                case "1W": interval = TimeSpan.FromDays(7); break;
                default: throw new ArgumentException("Invalid timeframe");
            }

            DateTime currentGroupStart = DateTime.MinValue;
            CandleData currentCandle = null;

            foreach (var candle in sourceCandles.OrderBy(c => c.Timestamp))
            {
                var candleTime = candle.Timestamp;
                var groupStart = GetGroupStartTime(candleTime, interval);

                if (groupStart != currentGroupStart)
                {
                    if (currentCandle != null) aggregated.Add(currentCandle);
                    currentGroupStart = groupStart;
                    currentCandle = new CandleData
                    {
                        Date = groupStart.ToString("yyyy-MM-dd"),
                        Time = groupStart.ToString("HH:mm"),
                        Open = candle.Open,
                        High = candle.High,
                        Low = candle.Low,
                        Close = candle.Close
                    };
                }
                else
                {
                    currentCandle.High = Math.Max(currentCandle.High, candle.High);
                    currentCandle.Low = Math.Min(currentCandle.Low, candle.Low);
                    currentCandle.Close = candle.Close;
                }
            }

            if (currentCandle != null) aggregated.Add(currentCandle);
            return aggregated;
        }

        private DateTime GetGroupStartTime(DateTime timestamp, TimeSpan interval)
        {
            if (interval.TotalMinutes >= 1440) // Daily or weekly
                return timestamp.Date;

            var totalMinutes = (int)(timestamp.TimeOfDay.TotalMinutes / interval.TotalMinutes) * interval.TotalMinutes;
            return timestamp.Date.AddMinutes(totalMinutes);
        }

        private void AddIndicator(PlotModel model, List<CandleData> candles, string indicator)
        {
            switch (indicator.Split(' ')[0])
            {
                case "SMA":
                    var period = int.Parse(indicator.Split(' ')[1]);
                    var smaValues = CalculateSMA(candles, period);
                    var smaSeries = new LineSeries
                    {
                        Title = $"SMA {period}",
                        Color = OxyColors.Blue
                    };
                    for (int i = 0; i < smaValues.Count; i++)
                        smaSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(candles[i + period - 1].Timestamp), smaValues[i]));
                    model.Series.Add(smaSeries);
                    break;

                    // Аналогично для других индикаторов
            }
        }

        private List<double> CalculateSMA(List<CandleData> candles, int period)
        {
            var smaValues = new List<double>();
            for (int i = period - 1; i < candles.Count; i++)
            {
                var sum = candles.Skip(i - period + 1).Take(period).Average(c => c.Close);
                smaValues.Add(sum);
            }
            return smaValues;
        }

        private void UpdateRecommendation(List<CandleData> candles)
        {
            if (candles.Count < 2) return;

            var lastCandle = candles.Last();
            var prevCandle = candles[^2];

            string recommendation;
            if (lastCandle.Close > lastCandle.Open && lastCandle.Close > prevCandle.Close)
            {
                recommendation = "ПОКУПАТЬ";
                L_RECOMMENDATION.ForeColor = Color.Green;
            }
            else
            {
                recommendation = "ПРОДАВАТЬ";
                L_RECOMMENDATION.ForeColor = Color.Red;
            }

            SafeUpdateLabel(L_RECOMMENDATION, recommendation);
        }

        private async void CB_INSTRUMENTS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CB_INSTRUMENTS.SelectedItem == null) return;

            try
            {
                var selectedInstrument = CB_INSTRUMENTS.SelectedItem.ToString();
                var selectedTimeFrame = CB_TIMEFRAME.SelectedItem.ToString();
                var collectionName = $"{selectedInstrument.Replace("/", "_")}_{selectedTimeFrame}";

                var database = _mongoClient.GetDatabase("CDA");
                _currentCollection = database.GetCollection<CandleData>(collectionName);

                await LoadDataAndPlot();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}

    [BsonIgnoreExtraElements]
    public class CandleData
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }

        [BsonIgnore]
        public DateTime Timestamp
        {
            get
            {
                if (DateTime.TryParseExact(
                    $"{Date} {Time}",
                    "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var result))
                {
                    return result;
                }
                return DateTime.MinValue;
            }
        }
    public CandleData Clone()
    {
        return new CandleData
        {
            Date = this.Date,
            Time = this.Time,
            Open = this.Open,
            High = this.High,
            Low = this.Low,
            Close = this.Close
        };
    }
}
