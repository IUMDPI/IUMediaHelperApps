using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using NLog.Config;
using Packager.Engine;
using Packager.Factories;
using Packager.Observers;
using Packager.Observers.LayoutRenderers;
using Packager.Processors;
using Packager.Providers;
using Packager.UserInterface;

namespace Packager
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
}