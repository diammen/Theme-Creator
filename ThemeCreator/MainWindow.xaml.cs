using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows.Markup;


namespace ThemeCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentFilePath = "";
        private const string hexPrefix = @"#";
        private List<Expander> expList = new List<Expander>();
        Theme theme = new Theme();
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
                _scenes.Add(new GameOverMenu());
            }

            public Theme(List<Scene> newScenes)
            {
                _scenes = newScenes;
            }
        }

        public class Scene : INotifyPropertyChanged
        {
            private string HexCode;
            private FontFamily _font;
            private Dictionary<string, string> _textColors;

            public string TextColor { get => HexCode; set => HexCode = value; }

            public Scene()
            {
                Font = new FontFamily("Resources/Acme 7 Wide.ttf#Grixel Acme 7 Wide");
            }

            public Dictionary<string, string> TextColors
            {
                get
                {
                    return _textColors;
                }
                set
                {
                    _textColors = value;
                    NotifyPropertyChanged("TextColors");
                }
            }

            public FontFamily Font
            {
                get
                {
                    return _font;
                }
                set
                {
                    _font = value;
                    NotifyPropertyChanged("Font");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public class MainMenu : Scene
        {
            public MainMenu()
            {
                TextColor = "FFFFFF";
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

            public InGame()
            {
                TextColors = new Dictionary<string, string>();

                TextColors.Add("PERFECT", "#FFFFFF");
                TextColors.Add("GREAT", "#FFFFFF");
                TextColors.Add("MISS", "#FFFFFF");
                TextColors.Add("SCORE", "#FFFFFF");
            }
        }

        public class GameOverMenu : Scene
        {
            public GameOverMenu()
            {
                TextColor = "FFFFFF";
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            expList.Add(mainMenuExpand);
            expList.Add(optionsExpand);
            expList.Add(ingameExpand);
            expList.Add(gameOverExpand);
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

            if (result == true)
            {
                FileStream temp = File.Create(dlg.FileName);
                temp.Close();

                currentFilePath = dlg.FileName;
                theme = new Theme();
                menuFont.DataContext = menuTextColor.DataContext = theme.Scenes[0];
                optionsFont.DataContext = optionsTextColor.DataContext = theme.Scenes[1];
                ingameFont.DataContext = perfectTextColor.DataContext = greatTextColor.DataContext = missTextColor.DataContext = scoreTextColor.DataContext = theme.Scenes[2];
                gameOverFont.DataContext = gameOverTextColor.DataContext = theme.Scenes[3];
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length > 0)
            {
                if (!textBox.Text.StartsWith(hexPrefix))
                {
                    textBox.Text = hexPrefix + textBox.Text;
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length > 0)
                if (textBox.SelectionStart <= 0)
                    textBox.SelectionStart = textBox.Text.Length;
        }

        private void PreviewInput(object sender, TextCompositionEventArgs e)
        {
            string pattern = @"^[#a-fA-F0-9]+$";
            Regex rgx = new Regex(pattern);

            TextBox textBox = (TextBox)sender;

            if (textBox.Text.Length > 0)
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
            // store entered values into theme object
            theme.Scenes[0].Font = new FontFamily(menuFont.Text);
            theme.Scenes[0].TextColor = menuTextColor.Text;
            theme.Scenes[1].Font = new FontFamily(optionsFont.Text);
            theme.Scenes[1].TextColor = optionsTextColor.Text;
            theme.Scenes[2].Font = new FontFamily(ingameFont.Text);
            theme.Scenes[2].TextColors["PERFECT"] = perfectTextColor.Text;
            theme.Scenes[2].TextColors["GREAT"] = greatTextColor.Text;
            theme.Scenes[2].TextColors["MISS"] = missTextColor.Text;
            theme.Scenes[2].TextColors["SCORE"] = scoreTextColor.Text;
            theme.Scenes[3].Font = new FontFamily(gameOverFont.Text);
            theme.Scenes[3].TextColor = gameOverTextColor.Text;

            string pattern = @"(Resources/[\w+\s+.ttf]+)";
            Regex rgx = new Regex(pattern);

            string input = "";

            input += "@Main Menu; font, font color\n";
            input += rgx.Match(theme.Scenes[0].Font.ToString()).ToString() + "\n";
            input += theme.Scenes[0].TextColor + "\n\n";
            input += "@Options Menu; font, font color\n";
            input += rgx.Match(theme.Scenes[1].Font.ToString()).ToString() + "\n";
            input += theme.Scenes[1].TextColor + "\n\n";
            input += "@In-game; font, perfect color, great color, miss color, score color\n";
            input += rgx.Match(theme.Scenes[2].Font.ToString()).ToString() + "\n";
            foreach (string color in theme.Scenes[2].TextColors.Values)
                input += color + "\n";
            input += "\n";
            input += "@Game Over Menu; font, font color\n";
            input += rgx.Match(theme.Scenes[3].Font.ToString()).ToString() + "\n";
            input += theme.Scenes[3].TextColor;

            using (FileStream file = File.OpenWrite(currentFilePath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(input);
                file.Write(info, 0, info.Length);
            }
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
                    theme.Scenes[0].Font = fontFam;
                }
                BindingExpression binding = menuFont.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
        }
        private void optionsFontBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "True Type Fonts (.ttf)|*.ttf";

            if (dlg.ShowDialog() == true)
            {

                var fonts = Fonts.GetFontFamilies(dlg.FileName);

                foreach (FontFamily fontFam in fonts)
                {
                    theme.Scenes[1].Font = fontFam;
                }
                BindingExpression binding = menuFont.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
        }
        private void ingameFontBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "True Type Fonts (.ttf)|*.ttf";

            if (dlg.ShowDialog() == true)
            {

                var fonts = Fonts.GetFontFamilies(dlg.FileName);

                foreach (FontFamily fontFam in fonts)
                {
                    theme.Scenes[2].Font = fontFam;
                }
                BindingExpression binding = menuFont.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
        }
        private void gameOverFontBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "True Type Fonts (.ttf)|*.ttf";

            if (dlg.ShowDialog() == true)
            {

                var fonts = Fonts.GetFontFamilies(dlg.FileName);

                foreach (FontFamily fontFam in fonts)
                {
                    theme.Scenes[3].Font = fontFam;
                }
                BindingExpression binding = menuFont.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
        }
        private void UIExpanded(object sender, RoutedEventArgs e)
        {
            Expander exp = (Expander)sender;

            foreach (Expander i in expList)
            {
                if (exp == i)
                {
                    i.IsExpanded = true;
                }
                else
                {
                    i.IsExpanded = false;
                }
            }
        }

        private void perfectColorGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox)sender;
            Binding binding = new Binding("Text");
            binding.Source = input;
            BindingOperations.SetBinding(timing, TextBlock.ForegroundProperty, binding);
            timing.Text = "PERFECT";
        }
        private void greatColorGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox)sender;
            Binding binding = new Binding("Text");
            binding.Source = input;
            BindingOperations.SetBinding(timing, TextBlock.ForegroundProperty, binding);
            timing.Text = "GREAT";
        }
        private void missColorGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox)sender;
            Binding binding = new Binding("Text");
            binding.Source = input;
            BindingOperations.SetBinding(timing, TextBlock.ForegroundProperty, binding);
            timing.Text = "MISS";
        }
    }
}
