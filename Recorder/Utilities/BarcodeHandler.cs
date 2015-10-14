using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Recorder.BarcodeScanner;
using Recorder.ViewModels;

namespace Recorder.Utilities
{
    public class BarcodeHandler : IDisposable
    {
        private readonly string[] _barcodeScannerIdentifiers;
        private readonly List<char> _buffer = new List<char>();
        private RawPresentationInput _input;
        private UserControlsViewModel _viewModel;

        public BarcodeHandler(string[] barcodeScannerIdentifiers)
        {
            _barcodeScannerIdentifiers = barcodeScannerIdentifiers;
        }

        public void Dispose()
        {
            _input?.Dispose();
        }

        public void Initialize(Visual client, UserControlsViewModel viewModel)
        {
            _viewModel = viewModel;
            _input = new RawPresentationInput(client, RawInputCaptureMode.Foreground);
            _input.KeyPressed += OnKeyPressed;
        }

        private async void OnKeyPressed(object sender, RawInputEventArgs e)
        {
            if (e.KeyPressState != KeyPressState.Down)
            {
                return;
            }

            if (TrapDeviceInput(e.Device?.Name) == false)
            {
                return;
            }

            e.Handled = true;

            if (e.Key == Key.Return)
            {
                await SetBarcode();
                _buffer.Clear();
            }
            else
            {
                _buffer.Add((char) e.VirtualKey);
            }
        }

        private bool TrapDeviceInput(string name)
        {
            return string.IsNullOrWhiteSpace(name) == false
                   && _barcodeScannerIdentifiers.Any(name.Contains);
        }

        private async Task SetBarcode()
        {
            var value = new string(_buffer.ToArray());
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (_viewModel.BarcodePanelViewModel.IsEnabled == false)
            {
                return;
            }

            _viewModel.BarcodePanelViewModel.Barcode = value;
            await _viewModel.ShowPanel<BarcodePanelViewModel>();
        }
    }
}