using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using AvaloniaApplication1;

public class TextRow : ObservableCollection<string>
{
    // This class represents a single row of characters
    // Inherits from ObservableCollection<string> where each string is a character
}

public class TextDocument : ObservableCollection<TextRow>
{
    // This represents your entire document (rows of characters)
    // Inherits from ObservableCollection<TextRow>
    
    public void AddNewRow()
    {
        Add(new TextRow());
    }
    
    public void AddCharacter(int rowIndex, string character)
    {
        if (rowIndex >= 0 && rowIndex < Count)
        {
            this[rowIndex].Add(character);
        }
    }
    
    public void SetCharacter(int rowIndex, int colIndex, string character)
    {
        if (rowIndex >= 0 && rowIndex < Count && 
            colIndex >= 0 && colIndex < this[rowIndex].Count)
        {
            this[rowIndex][colIndex] = character;
        }
    }
}
public class BlockEditor : UserControl
{
    private readonly TextDocument _document = new TextDocument();
    
    public BlockEditor()
    {
        void LoadSampleText()
        {
            // Clear existing content
            _document.Clear();
    
            // Sample text - replace with your actual Lorem Ipsum or other text
            var sampleText = Lorem.Text;
    
            // Split into lines and populate the editor
            var lines = sampleText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    
            foreach (var line in lines)
            {
                // Add new row
                _document.AddNewRow();
                var currentRowIndex = _document.Count - 1;
        
                // Add each character to the row
                foreach (var character in line)
                {
                    _document.AddCharacter(currentRowIndex, character.ToString());
                }
            }
        }
        LoadSampleText();
        // Create the outer ItemsRepeater (for rows)
        var rowsRepeater = new ItemsRepeater
        {
            ItemsSource = _document,
            ItemTemplate = new FuncDataTemplate<TextRow>((row, _) => 
            {
                // Create inner ItemsRepeater for each row (characters)
                return new ItemsRepeater
                {
                    ItemsSource = row,
                    ItemTemplate = new FuncDataTemplate<string>((character, _) => 
                        new TextBlock 
                        { 
                            Text = character,
                            //Margin = new Thickness(2)
                        }),
                    Layout = new StackLayout
                    {
                        Orientation = Orientation.Horizontal,
                        //Spacing = 2
                    }
                };
            }),
            Layout = new StackLayout
            {
                Orientation = Orientation.Vertical,
                //Spacing = 4
            }
        };
        
        Content = new ScrollViewer { Content = rowsRepeater, HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalScrollBarVisibility = ScrollBarVisibility.Visible };
        
        // Add initial empty row
        _document.AddNewRow();
    }
    
    // Example usage methods
    public void AddCharacter(int rowIndex, string character)
    {
        _document.AddCharacter(rowIndex, character);
    }
    
    public void SetCharacter(int rowIndex, int colIndex, string character)
    {
        _document.SetCharacter(rowIndex, colIndex, character);
    }
    
    public void AddNewRow()
    {
        _document.AddNewRow();
    }
}