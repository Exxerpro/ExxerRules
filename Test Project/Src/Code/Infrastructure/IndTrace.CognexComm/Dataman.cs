// <copyright file="Dataman.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.CognexComm;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Xml;
using Cognex.DataMan.SDK;
using Cognex.DataMan.SDK.Discovery;
using Cognex.DataMan.SDK.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Cognex.DataMan.SDK.Discovery.EthSystemDiscoverer;

using Logging = Microsoft.Extensions.Logging;

/// <summary>
/// Provides communication functionality for Cognex DataMan barcode readers.
/// </summary>
public class DataMan
{
    // Use like this to subscribe
    //  dataMan.BarCode.Subscribe(_ => FUNC<,> );
    private readonly Subject<DataMan> subject = new();

    /// <summary>
    /// Gets an observable that emits when a barcode is scanned.
    /// </summary>
    public IObservable<DataMan> BarCode => this.subject.AsObservable();

    /// <summary>
    /// Gets or sets a value indicating whether the DataMan is connected.
    /// </summary>
    public bool IsConnected;

    /// <summary>
    /// Gets the list of captured images.
    /// </summary>
    public List<Image> ListImages { get; private set; }

    /// <summary>
    /// Gets the list of captured strings.
    /// </summary>
    public List<string> ListStrings { get; }

    /// <summary>
    /// Gets the current result from the DataMan.
    /// </summary>
    public string? Result { get; private set; }

    private readonly object currentResultInfoSyncLock = new();
    private readonly Logging.ILogger logger;

    // [Fix]
    // CLAUDE
    // Date: 02/09/2025
    // Reason: [Nullable Reference] - Make fields nullable to fix CS8618 and CS8625 warnings
    private ISystemConnector? connector = null;

    private EthSystemDiscoverer? ethSystemDiscoverer = null;
    private ResultCollector? resultsCollector;
    private DataManSystem? dataManSystem = null;

    // Declare the delegate (if using non-generic pattern).
    private delegate void DataManDataReceivedHandler(object sender, EventArgs e);

    // Declare the event.
    private event DataManDataReceivedHandler? DataManDataReceived;

    // [Fix]
    // CLAUDE
    // Date: 02/09/2025
    // Reason: [Nullable Reference] - Make properties nullable to fix CS8618 warnings or provide proper initialization

    /// <summary>
    /// Gets the IP address of the DataMan.
    /// </summary>
    public IPAddress? IpAddress { get; private set; }

    /// <summary>
    /// Gets the password for DataMan authentication.
    /// </summary>
    public string? Password { get; private set; }

    /// <summary>
    /// Gets the username for DataMan authentication.
    /// </summary>
    public string? UserName { get; private set; }

    private IAsyncResult? AsyncResult { get; set; }

    private const bool Autoconnect = true;

    private SystemInfo? InfoConnectionDataMan { get; set; }

    private List<SystemInfo> ListDiscoveredItems { get; set; } = [];

    /// <summary>
    /// Gets the IP address.
    /// </summary>
    public IPAddress? Ip { get; }

    /// <summary>
    /// Gets the username.
    /// </summary>
    public string? User { get; }

    /// <summary>
    /// Gets the password.
    /// </summary>
    public string? Pwd { get; }

    private DataManSettings? dataManSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataMan"/> class with logger and settings.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="dataManSettings">The DataMan configuration settings.</param>
    public DataMan(ILogger<DataMan> logger, IOptions<DataManSettings> dataManSettings)
    {
        this.dataManSettings = dataManSettings.Value;

        this.ConfigNetwork(this.dataManSettings);
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // ListDiscoveredItems is already initialized above with = []
        this.ListImages = [];
        this.ListStrings = [];
        this.Result = string.Empty;

        // Create discoverers to discover ethernet systems.
        this.ethSystemDiscoverer = new EthSystemDiscoverer();

        // Subscribe to the system discovered event.
        this.ethSystemDiscoverer.SystemDiscovered += new SystemDiscoveredHandler(this.OnEthSystemDiscovered);
    }

    /// <summary>
    /// Configures the network settings for the DataMan.
    /// </summary>
    /// <param name="dataManSettings">The DataMan configuration settings.</param>
    public void ConfigNetwork(DataManSettings dataManSettings)
    {
        this.UserName = this.dataManSettings?.UserName ?? throw new ArgumentNullException(nameof(DataMan.dataManSettings.UserName));
        this.Password = this.dataManSettings?.Password ?? throw new ArgumentNullException(nameof(DataMan.dataManSettings.Password));

        this.IpAddress = this.dataManSettings?.IpAddress != null ? IPAddress.Parse(this.dataManSettings.IpAddress) : throw new ArgumentNullException(nameof(DataMan.dataManSettings.IpAddress));
        this.dataManSettings = dataManSettings;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataMan"/> class with logger.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public DataMan(Logging.ILogger logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // ListDiscoveredItems is already initialized above with = []
        this.ListImages = [];
        this.ListStrings = [];
        this.Result = string.Empty;

        // Create discoverers to discover ethernet systems.
        this.ethSystemDiscoverer = new EthSystemDiscoverer();

        // Subscribe to the system discovered event.
        this.ethSystemDiscoverer.SystemDiscovered += new SystemDiscoveredHandler(this.OnEthSystemDiscovered);

        this.ethSystemDiscoverer.Discover();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataMan"/> class with logger and connection parameters.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="ip">The IP address.</param>
    /// <param name="user">The username.</param>
    /// <param name="pwd">The password.</param>
    public DataMan(Logging.ILogger logger, IPAddress ip, string user, string pwd)
        : this(logger)
    {
        this.Ip = ip;
        this.User = user;
        this.Pwd = pwd;
    }

    /// <summary>
    /// Connects to the DataMan using the specified IP address and credentials.
    /// </summary>
    /// <param name="ip">The IP address to connect to.</param>
    /// <param name="user">The username for authentication.</param>
    /// <param name="pwd">The password for authentication.</param>
    public void Connect(IPAddress ip, string user, string pwd)
    {
        this.ethSystemDiscoverer?.Discover();
    }

    /// <summary>
    /// Connects to the DataMan using default settings.
    /// </summary>
    public void Connect()
    {
        this.ethSystemDiscoverer?.Discover();
    }

    private void BarCodeChanged()
    {
        this.subject.OnNext(this); // Raise event
    }

    private void CallBackStringResult(IAsyncResult ar)
    {
        throw new NotImplementedException();
    }

    private void CleanupConnection()
    {
        if (this.dataManSystem != null)
        {
            // UnSubscribe to events that are signalled when the system is connected / disconnected.
            this.dataManSystem.SystemConnected -= this.OnSystemConnected;
            this.dataManSystem.SystemDisconnected -= this.OnSystemDisconnected;
            this.dataManSystem.SystemWentOnline -= this.OnSystemWentOnline;
            this.dataManSystem.SystemWentOffline -= this.OnSystemWentOffline;
            this.dataManSystem.KeepAliveResponseMissed -= this.OnKeepAliveResponseMissed;
            this.dataManSystem.BinaryDataTransferProgress -= this.OnBinaryDataTransferProgress;
            this.dataManSystem.OffProtocolByteReceived -= this.OffProtocolByteReceived;
            this.dataManSystem.AutomaticResponseArrived -= this.OnAutomaticResponseArrived;
            this.dataManSystem.ImageArrived -= this.OnImageArrived;
            this.dataManSystem.ImageGraphicsArrived -= this.OnImageGraphicsArrived;
            this.dataManSystem.ReadStringArrived -= this.OnXmlResultArrived;
            this.dataManSystem.ReadStringArrived -= this.OnReadStringArrived;
        }

        if (this.resultsCollector != null)
        {
            this.resultsCollector.ComplexResultCompleted -= this.OnComplexResultCompleted;
            this.resultsCollector.SimpleResultDropped += this.OnSimpleResultDropped;
            this.resultsCollector.ClearCachedResults();
        }

        this.resultsCollector = null;
        this.dataManSystem = null;
        this.connector = null;
    }

    private void ConnectDataMan(SystemInfo systemInfo)
    {
        try
        {
            var conn = new EthSystemConnector(systemInfo.IPAddress, systemInfo.Port)
            {
                UserName = this.UserName ?? string.Empty,
                Password = this.Password ?? string.Empty,
            };

            this.connector = conn;

            this.dataManSystem = new DataManSystem(this.connector)
            {
                DefaultTimeout = 5000,
            };

            // Subscribe to events that are signalled when the system is connected / disconnected.
            this.dataManSystem.SystemConnected += new SystemConnectedHandler(this.OnSystemConnected);
            this.dataManSystem.SystemDisconnected += new SystemDisconnectedHandler(this.OnSystemDisconnected);
            this.dataManSystem.SystemWentOnline += new SystemWentOnlineHandler(this.OnSystemWentOnline);
            this.dataManSystem.SystemWentOffline += new SystemWentOfflineHandler(this.OnSystemWentOffline);
            this.dataManSystem.KeepAliveResponseMissed += new KeepAliveResponseMissedHandler(this.OnKeepAliveResponseMissed);
            this.dataManSystem.BinaryDataTransferProgress +=
                new BinaryDataTransferProgressHandler(this.OnBinaryDataTransferProgress);
            this.dataManSystem.OffProtocolByteReceived += new OffProtocolByteReceivedHandler(this.OffProtocolByteReceived);
            this.dataManSystem.AutomaticResponseArrived +=
                new AutomaticResponseArrivedHandler(this.OnAutomaticResponseArrived);
            this.dataManSystem.ImageArrived += new ImageArrivedHandler(this.OnImageArrived);
            this.dataManSystem.ImageGraphicsArrived += new ImageGraphicsArrivedHandler(this.OnImageGraphicsArrived);
            this.dataManSystem.XmlResultArrived += new XmlResultArrivedHandler(this.OnXmlResultArrived);
            this.dataManSystem.ReadStringArrived += new ReadStringArrivedHandler(this.OnReadStringArrived);

            // Subscribe to events that are signalled when the device sends auto-responses.
            const ResultTypes requestedResultTypes =
                ResultTypes.ReadXml | ResultTypes.Image | ResultTypes.ImageGraphics;
            this.resultsCollector = new ResultCollector(this.dataManSystem, requestedResultTypes);
            this.resultsCollector.ComplexResultCompleted += this.OnComplexResultCompleted;
            this.resultsCollector.SimpleResultDropped += this.OnSimpleResultDropped;

            this.dataManSystem.SetKeepAliveOptions(true, 3000, 1000);

            try
            {
                this.dataManSystem.Connect();

                this.logger.LogInformation("Connection a DataMan {0}", systemInfo.IPAddress);

                this.dataManSystem.SetResultTypes(requestedResultTypes);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while connecting DataMan");
                throw;
            }
        }
        catch (Exception ex)
        {
            this.CleanupConnection();
            this.logger.LogError(ex, "Error while disconnecting DataMan");
            throw;
        }
    }

    private void DisConnectDataMan()
    {
        try
        {
            if (this.dataManSystem?.State != ConnectionState.Connected)
            {
                this.dataManSystem?.Disconnect();
            }

            if (this.dataManSystem == null)
            {
                this.CleanupConnection();
            }

            this.logger.LogInformation("disconnecting DataMan");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error while disconnecting DataMan");
            throw;
        }
    }

    private void GetComplexResults(ComplexResult complexResult)
    {
        var resultId = -1;
        var collectedResults = ResultTypes.None;

        // Take a reference or copy values from the locked result info object. This is done so
        // that the lock is used only for a short period of time.
        lock (this.currentResultInfoSyncLock)
        {
            foreach (var simpleResult in complexResult.SimpleResults)
            {
                collectedResults |= simpleResult.Id.Type;

                switch (simpleResult.Id.Type)
                {
                    case ResultTypes.Image:
                        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                        {
                            var image = ImageArrivedEventArgs.GetImageFromImageBytes(simpleResult.Data);
                            if (image != null)
                            {
                                this.ListImages.Add(image);
                            }
                        }
                        break;

                    case ResultTypes.ImageGraphics:
                        this.ListStrings.Add(simpleResult.GetDataAsString());
                        break;

                    case ResultTypes.ReadXml:
                        this.Result = this.GetReadStringFromResultXml(simpleResult.GetDataAsString());
                        resultId = simpleResult.Id.Id;

                        break;

                    case ResultTypes.ReadString:
                        this.Result = simpleResult.GetDataAsString();
                        resultId = simpleResult.Id.Id;
                        break;

                    case ResultTypes.None:
                        break;

                    case ResultTypes.XmlStatistics:
                        break;

                    case ResultTypes.TrainingResults:
                        break;

                    case ResultTypes.CodeQualityData:
                        break;

                    case ResultTypes.ReadXmlExtended:
                        break;

                    case ResultTypes.InputEvent:
                        break;

                    case ResultTypes.GroupTriggering:
                        break;

                    case ResultTypes.ProcessControlMetricsReport:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // Raise the event in a thread-safe manner using the ?. operator.
        this.DataManDataReceived?.Invoke(this, new EventArgs());
        this.BarCodeChanged();

        this.logger.LogInformation("Complex result arrived: resultId = {ResultId}, read result = {Result}", resultId, this.Result);
        this.logger.LogInformation("Complex result contains: {Results}", collectedResults.ToString());
    }

    private string? GetReadStringFromResultXml(string resultXml)
    {
        try
        {
            var doc = new XmlDocument();

            doc.LoadXml(resultXml);

            var fullStringNode = doc.SelectSingleNode("result/general/full_string");

            if (!(fullStringNode != null && this.dataManSystem != null &&
                  this.dataManSystem.State == ConnectionState.Connected))
            {
                return null;
            }

            // [Fix]
            // CLAUDE
            // Date: 02/09/2025
            // Reason: [Nullable Reference] - Add null check for fullStringNode.Attributes to fix CS8602
            var encoding = fullStringNode.Attributes?["encoding"];
            if (encoding == null || encoding.InnerText != "base64")
            {
                return fullStringNode.InnerText;
            }

            if (string.IsNullOrEmpty(fullStringNode.InnerText))
            {
                return fullStringNode.InnerText;
            }

            var code = Convert.FromBase64String(fullStringNode.InnerText);

            return this.dataManSystem.Encoding.GetString(code, 0, code.Length);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error while Reading Xml from DataMan");
            throw;
        }
    }

    private void OffProtocolByteReceived(object sender, OffProtocolByteReceivedEventArgs args)
    {
        this.logger.LogInformation("OffProtocolByteReceived: {Byte}", (char)args.Byte);
    }

    private void OnAutomaticResponseArrived(object sender, AutomaticResponseArrivedEventArgs args)
    {
        this.logger.LogInformation(
            "AutomaticResponseArrived: Type={Type}, RecipeId={ResponseId}, Data={DataLength} bytes",
            args.DataType.ToString(), args.ResponseId, args.Data?.Length ?? 0);
    }

    private void OnBinaryDataTransferProgress(object sender, BinaryDataTransferProgressEventArgs args)
    {
        this.logger.LogInformation(
            "OnBinaryDataTransferProgress: Direction={Direction}, Progress={ProgressPercent}%, TotalBytes={TotalBytes}, Type={ResultType}, RecipeId={RecipeId}",
            args.Direction == TransferDirection.Incoming ? "Receiving" : "Sending",
            args.TotalDataSize > 0 ? (int)(100 * (args.BytesTransferred / (double)args.TotalDataSize)) : -1,
            args.TotalDataSize,
            args.ResultType.ToString(),
            args.ResponseId);
    }

    private void OnComplexResultCompleted(object sender, ComplexResult e)
    {
        this.GetComplexResults(e);
    }

    private void OnEthSystemDiscovered(SystemInfo systemInfo)
    {
        this.ListDiscoveredItems.Add(systemInfo);

        if (this.IpAddress?.Equals(systemInfo.IPAddress) == true)
        {
            this.ConnectDataMan(systemInfo);
        }
    }

    private void OnImageArrived(object sender, EventArgs args)
    {
        try
        {
            // var image = _dataManSystem.EndGetLiveImage(AsyncResult);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "OnImageArrived Error");
        }

        // Raise the event in a thread-safe manner using the ?. operator.
        this.DataManDataReceived?.Invoke(this, new EventArgs());
        this.BarCodeChanged();
    }

    private void OnImageGraphicsArrived(object sender, EventArgs args)
    {
        try
        {
            // var image = _dataManSystem.EndGetLiveImage(AsyncResult);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "OnLiveImageArrived Error");
        }

        // Raise the event in a thread-safe manner using the ?. operator.
        this.DataManDataReceived?.Invoke(this, new EventArgs());
        this.BarCodeChanged();
    }

    private void OnKeepAliveResponseMissed(object sender, EventArgs args)
    {
        this.logger.LogInformation("Keep-alive response missed");
    }

    private void OnReadStringArrived(object sender, EventArgs args)
    {
        try
        {
            var stringresult = this.dataManSystem?.BeginGetLiveImage(imageFormat: ImageFormat.bitmap, imageSize: ImageSize.Full, imageQuality: ImageQuality.Low, callback: this.CallBackStringResult, state: new object());
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "OnReadStringArrived Error");
        }

        // Raise the event in a thread-safe manner using the ?. operator.
        this.DataManDataReceived?.Invoke(this, new EventArgs());
        this.BarCodeChanged();
    }

    private void OnSimpleResultDropped(object sender, SimpleResult e)
    {
        this.logger.LogInformation("System disconnected {}", e);
    }

    private void OnSystemConnected(object sender, EventArgs args)
    {
        this.logger.LogInformation("System connected");
        this.IsConnected = true;
    }

    private void OnSystemDisconnected(object sender, EventArgs args)
    {
        this.logger.LogInformation("System disconnected from {Sender}, Args: {Args}", sender, args);

        this.IsConnected = false;

        // TODO
    }

    private void OnSystemWentOffline(object sender, EventArgs args)
    {
        this.logger.LogInformation("System went offline");
    }

    private void OnSystemWentOnline(object sender, EventArgs args)
    {
        this.logger.LogInformation("System went online");
    }

    private void OnXmlResultArrived(object sender, EventArgs args)
    {
        try
        {
            // var image = _dataManSystem.EndGetLiveImage(AsyncResult);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "OnXmlResultArrived Error");
        }

        // Raise the event in a thread-safe manner using the ?. operator.
        this.DataManDataReceived?.Invoke(this, new EventArgs());
        this.BarCodeChanged();
    }
}// Class

// NameSpace
