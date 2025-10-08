// <copyright file="IndTraceTagRx.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.S7Rx;

/// <summary>
/// Represents the IndTraceTagRx.
/// </summary>
public class IndTraceTagRx : IIndTraceTagRx
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndTraceTagRx"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="netType">The netType.</param>
    /// <param name="enableSimulation">The enableSimulation.</param>
    public IndTraceTagRx(Type netType, bool enableSimulation = true)
    {
        this.NetType = netType;
        this.EnableSimulation = enableSimulation;
    }

    /// <summary>
    /// Gets or sets the Variable.
    /// </summary>
    public Variable Variable { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the EnableSimulation.
    /// </summary>
    public bool EnableSimulation { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndTraceTagRx"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="variable">The variable.</param>
    /// <param name="enableSimulation">The enableSimulation.</param>
    public IndTraceTagRx(Variable variable, bool enableSimulation = false)
    {
        this.EnableSimulation = enableSimulation;
        this.Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        var netTypeName = this.Variable.NetType;
        if (string.IsNullOrWhiteSpace(netTypeName))
        {
            throw new ArgumentException("Variable.NetType cannot be null or empty", nameof(variable));
        }
        Type? resolvedType = Type.GetType(netTypeName, throwOnError: true, ignoreCase: true);
        if (resolvedType is null)
        {
            throw new InvalidOperationException($"Unable to resolve NetType '{netTypeName}'");
        }
        this.netType = resolvedType;
    }

    private Type netType = typeof(string);

    /// <inheritdoc/>
    public Type NetType
    {
        get => this.netType;
        set
        {
            if (value == null)
            {
                throw new Exception("NetType cant be null");
            }

            if (!S7Types.Types().ContainsKey(value.ToString()))
            {
                throw new Exception("Invalid data type");
            }

            this.netType = value;
        }
    }

    private object value = "0";

    /// <inheritdoc/>
    public object Value
    {
        get
        {
            // Guard rails: value and netType are initialized in ctor/fields

            // if (EnableSimulation) return _value;

            // dynamic val = _controller.GetValue<dynamic>(_variable.Alias);
            this.value = Convert.ChangeType(this.value, this.netType);
            return this.value;
        }

        set
        {
            if (value is null)
            {
                return;
            }

            dynamic val = Convert.ChangeType(value, this.netType);
            this.value = val;

            if (this.EnableSimulation)
            {
                return;
            }

            // _controller.SetValue<dynamic>(_variable.Alias, val);
        }
    }

    /// <summary>
    /// Executes ToString operation.
    /// </summary>
    /// <returns>The result of ToString.</returns>
    public override string ToString()
    {
        dynamic value = Convert.ChangeType(this.value, this.netType);
        return $"{value}";
    }

    /// <summary>
    /// Executes DownloadValueAsync operation.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <returns>The result of DownloadValueAsync.</returns>
    public async Task<bool> DownloadValueAsync(IPlc controller)
    {
        var connected = await controller.EnsureIsConnected();

        if (this.netType == null)
        {
            throw new Exception("GetValueAsync NetType can not be null");
        }

        if (this.EnableSimulation)
        {
            return false;
        }

        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller), "SetValueAsync controller can not be null");
        }

        if (this.Value == null)
        {
            throw new ArgumentNullException(nameof(this.Value), "SetValueAsync ValueString can not  be null");
        }

        if (this.netType == typeof(short))
        {
            if (this.Value != null)
            {
                var value = (short)Convert.ChangeType(this.Value, typeof(short));
                await controller.SetValue<short>(this.Variable.Alias, value).ConfigureAwait(false);
            }
        }

        if (this.netType == typeof(int))
        {
            if (this.Value != null)
            {
                var value = (int)Convert.ChangeType(this.Value, typeof(int));
                await controller.SetValue<int>(this.Variable.Alias, value).ConfigureAwait(false);
            }
        }

        if (this.netType == typeof(string))
        {
            var stringValue = Convert.ToString(this.Value) ?? string.Empty;
            await controller.SetValue<string>(this.Variable.Alias, stringValue).ConfigureAwait(false);
        }

        if (this.netType == typeof(float))
        {
            if (this.Value != null)
            {
                var value = (float)Convert.ChangeType(this.Value, typeof(float));
                await controller.SetValue<float>(this.Variable.Alias, value).ConfigureAwait(false);
            }

            return true;
        }

        return connected;
    }

    /// <summary>
    /// Executes UploadValueAsync operation.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <returns>The result of UploadValueAsync.</returns>
    public async Task<string> UploadValueAsync(IPlc controller)
    {
        var connected = await controller.EnsureIsConnected();

        if (this.netType == null)
        {
            throw new Exception("UploadValueAsync NetType can not be null");
        }

        if (this.EnableSimulation)
        {
            return string.Empty;
        }

        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller), "UploadValueAsync controller can not be null");
        }

        if (this.netType == typeof(short))
        {
            this.value = (short)await controller.GetValue<short>(this.Variable.Alias).ConfigureAwait(false);
            return Convert.ToString(this.value) ?? string.Empty;
        }

        if (this.netType == typeof(int))
        {
            this.value = (int)await controller.GetValue<int>(this.Variable.Alias).ConfigureAwait(false);
            return Convert.ToString(this.value) ?? string.Empty;
        }

        if (this.netType == typeof(string))
        {
            this.value = (string)await controller.GetValue<string>(this.Variable.Alias).ConfigureAwait(false);
            return Convert.ToString(this.value) ?? string.Empty;
        }

        if (this.netType == typeof(float))
        {
            this.value = (float)await controller.GetValue<float>(this.Variable.Alias).ConfigureAwait(false);
            return Convert.ToString(this.value) ?? string.Empty;
        }

        if (this.netType == typeof(bool))
        {
            this.value = (bool)await controller.GetValue<bool>(this.Variable.Alias).ConfigureAwait(false);
            return Convert.ToString(this.value) ?? string.Empty;
        }

        return connected ? "Type not Found" : "Unknown Error";
    }
}

public static class ControllerExtensions
{
    public static async Task<bool> EnsureIsConnected(this IPlc controller)
    {
        var connected = false;
        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        var subscription = controller.ConnectionState.Subscribe(connectionState =>
        {
            if (connectionState == ConnectionState.Connected)
            {
                connected = true;
            }
        });

        if (connected)
        {
            return true;
        }

        await controller.InitializeConnection();
        subscription.Dispose();

        return connected;
    }
}

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IndTraceTagRx logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
