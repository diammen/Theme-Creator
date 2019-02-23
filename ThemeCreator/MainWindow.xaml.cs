using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;
using System.Windows.Markup;


namespace ThemeCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string hexPrefix = @"#";
        public class Theme
        {
            private List<Scene> _scenes;
  
            public List<Scene> Scenes { get => _scenes; set => _scenes = value; }

            public Theme()
            {
                _scenes = new List<Scene>();
                _scenes.Add(new MainMenu());
                _scenes.Add(new OptionsMenu());
                _scenes.Add(new InGame());
            }

            public Theme(List<Scene> newScenes)
            {
                _scenes = newScenes;
            }
        }

        public class Scene
        {
            private string HexCode;
            private FontFamily _font;

            public string TextColor { get => HexCode; set => HexCode = value; }

            public FontFamily Font { get => _font; set => _font = value; }
        }

        public class MainMenu : Scene
        {
            public MainMenu()
            {
                TextColor = "FFFFFF";
                Font = new FontFamily("Resources/#Grixel Acme 7 Wide");
            }
        }

        public class OptionsMenu : Scene
        {
            public OptionsMenu()
            {
                TextColor = "FFFFFF";
            }
        }

        public class InGame : Scene
        {
            private string perfectColor;
            private string greatColor;
            private string missColor;
            private string scoreColor;

            private Dictionary<string, string> _textColors;

            public Dictionary<string, string> TextColors { get => _textColors; set => _textColors = value; }

            public InGame()
            {
                _textColors = new Dictionary<string, string>();
            }
        }
        Theme test = new Theme();
        public MainWindow()
        {
            InitializeComponent();

            menuTextColor.DataContext = test.Scenes[0];
            optionsTextColor.DataContext = test.Scenes[1];
            menuFont.DataContext = test.Scenes[0];
    }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "uso!mania themes (.uso)|*.uso";
            if (dlg.ShowDialog() == true)
            {

            }
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "theme1";
            dlg.DefaultExt = ".uso";
            dlg.Filter = "uso!mania themes (.uso)|*.uso";

            Nullable<bool> result = dlg.ShowDialog();

            string input = "";

            input += "Main Menu: " + menuTextColor.Text + "\n";
            input += "Options Menu: " + optionsTextColor.Text + "\n";

            if (result == true)
            {
                using (FileStream file = File.Create(dlg.FileName))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(input);
                    file.Write(info, 0, info.Length);
                }
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!textBox.Text.StartsWith(hexPrefix))
            {
                textBox.Text = hexPrefix + textBox.Text;
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.SelectionStart <= 0)
            textBox.SelectionStart = textBox.Text.Length;
        }

        private void PreviewInput(object sender, TextCompositionEventArgs e)
        {
            string pattern = @"^[#a-fA-F0-9]+$";
            Regex rgx = new Regex(pattern);

            TextBox textBox = (TextBox)sender;
            if (!rgx.IsMatch(e.Text) || textBox.Text.Length >= 7)
                e.Handled = true;
        }

        private void TextBoxLoaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = "FFFFFF";
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mainFontBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "True Type Fonts (.ttf)|*.ttf";
            if (dlg.ShowDialog() == true)
            {

                var fonts = Fonts.GetFontFamilies(dlg.FileName);

                foreach(FontFamily fontFam in fonts)
                {
                    test.Scenes[0].Font = fontFam;
                }

                menuFont.DataContext = test.Scenes[0];
            }
        }
    }
}
