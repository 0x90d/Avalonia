﻿using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Gpu;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Avalonia.Windowing.Bindings;

namespace Avalonia.Windowing
{
    public class WindowImpl : IWindowImpl
    {
        IWindowWrapper _windowWrapper;
        private LogicalPosition _lastPosition;

        public WindowImpl(IWindowWrapper wrapper)
        {
            _windowWrapper = wrapper;
        }

        public WindowState WindowState { get; set; }
        public Action<WindowState> WindowStateChanged { get; set; }
        public Func<bool> Closing { get; set; }
        public Point Position
        {
            get
            {
                var (x, y) = _windowWrapper.GetPosition();
                return new Point(x, y);
            }
            set
            {
                var x = value;
            }
        }

        public Action<Point> PositionChanged { get; set; }
        public Action Deactivated { get; set; }
        public Action Activated { get; set; }

        public IPlatformHandle Handle => new DummyPlatformHandle();

        public Size MaxClientSize => Size.Empty;

        public IScreenImpl Screen => new Monitors();

        public Size ClientSize
        {
            get
            {
                var (width, height) = _windowWrapper.GetSize();
                return new Size(width, height);
            }
        }

        public double Scaling => _windowWrapper.GetScaleFactor();

        public IEnumerable<object> Surfaces => new List<object>() { _windowWrapper };

        public Action<RawInputEventArgs> Input { get; set; }
        public Action<Rect> Paint { get; set; }
        public Action<Size> Resized { get; set; }
        public Action<double> ScalingChanged { get; set; }
        public Action Closed { get; set; }

        public IMouseDevice MouseDevice => new MouseDevice();

        public void Activate()
        {
        }

        public void BeginMoveDrag()
        {
        }

        public void BeginResizeDrag(WindowEdge edge)
        {
        }

        public void CanResize(bool value)
        {
        }

        public IRenderer CreateRenderer(IRenderRoot root)
        {
            return new ImmediateRenderer(root);
        }

        public void Dispose()
        {
            _windowWrapper.Dispose();
        }

        public void Hide()
        {
            _windowWrapper.Hide();
        }

        public void Invalidate(Rect rect)
        {
            Paint?.Invoke(rect);
        }

        public Point PointToClient(Point point)
        {
            var position = Position;
            return new Point(point.X + position.X, point.Y + position.Y);
        }

        public Point PointToScreen(Point point)
        {
            var position = Position;
            return new Point(point.X - position.X, point.Y - position.Y); ;
        }

        public void Resize(Size clientSize)
        {
            if (clientSize == ClientSize)
                return;

            // This is where we size the window accordingly..
            _windowWrapper.SetSize(clientSize.Width, clientSize.Height);
            //Resized(clientSize);
        }

        public void SetCursor(IPlatformHandle cursor)
        {
        }

        public void SetIcon(IWindowIconImpl icon)
        {
        }

        private IInputRoot _inputRoot;
        public void SetInputRoot(IInputRoot inputRoot) => _inputRoot = inputRoot;

        public void SetMinMaxSize(Size minSize, Size maxSize)
        {
        }

        public void SetSystemDecorations(bool enabled)
        {
            _windowWrapper.ToggleDecorations(enabled);
        }

        public void SetTitle(string title)
        {
            _windowWrapper.SetTitle(title);
        }

        public void SetTopmost(bool value)
        {
        }

        public void Show()
        {
            _windowWrapper.Show();
        }

        public IDisposable ShowDialog()
        {
            return null;
        }

        public void ShowTaskbarIcon(bool value)
        {
        }

        public void OnKeyboardEvent (KeyboardEvent evt)
        {

        }

        public void OnMouseEvent(MouseEvent evt)
        {
            Dispatcher.UIThread.RunJobs(DispatcherPriority.Input);

            switch(evt.EventType)
            {
                case MouseEventType.Move:
                    _lastPosition = evt.Position;
                    break;

                case MouseEventType.Wheel:
                    Input(new RawMouseWheelEventArgs(MouseDevice, (uint)Environment.TickCount, _inputRoot, new Point(_lastPosition.X, _lastPosition.Y), new Point(evt.Position.X / 50, evt.Position.Y / 50), InputModifiers.None));
                    return;
            }

            Input(new RawMouseEventArgs(MouseDevice, (uint)Environment.TickCount, _inputRoot, (RawMouseEventType)evt.EventType, new Point(_lastPosition.X, _lastPosition.Y), InputModifiers.None));
        }

        public void OnResizeEvent(ResizeEvent evt)
        {
            Resized?.Invoke(ClientSize);
            if (_windowWrapper is IGpuContext gpuCtx) {
                gpuCtx.ResizeContext(ClientSize.Width, ClientSize.Height);
            }
        }
    }
}