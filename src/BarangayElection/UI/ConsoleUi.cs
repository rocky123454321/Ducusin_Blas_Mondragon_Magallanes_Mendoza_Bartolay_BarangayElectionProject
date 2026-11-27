namespace BarangayElection.UI;

internal readonly record struct UiLine(string Text, ConsoleColor Color = ConsoleColor.Gray);

internal readonly record struct StatCard(string Label, string Value, string Note, ConsoleColor Color);

internal sealed class ConsoleUi
{
    public const int Width = 79;
    private const ConsoleColor Border = ConsoleColor.DarkGray;

    public void Clear()
    {
        try { Console.Clear(); }
        catch (IOException) { }
    }

    public void Header(string title, string subtitle)
    {
        Console.WriteLine();
        WriteLine("  " + title, ConsoleColor.Cyan);
        WriteLine("  " + subtitle, ConsoleColor.DarkGray);
        Console.WriteLine();
    }

    public void BlankLine() => Console.WriteLine();

    public void StatCards(IReadOnlyList<StatCard> cards)
    {
        const int gap = 1;
        int cardWidth = (Width - gap * (cards.Count - 1)) / cards.Count;
        int inner = cardWidth - 2;

        CardBorderRow(cards.Count, "┌", "┐", inner);
        CardTextRow(cards, cardWidth, c => c.Label, _ => ConsoleColor.DarkGray);
        CardTextRow(cards, cardWidth, c => c.Value, c => c.Color);
        CardTextRow(cards, cardWidth, c => c.Note, _ => ConsoleColor.DarkGray);
        CardBorderRow(cards.Count, "└", "┘", inner);
    }

    public void Panel(string title, IEnumerable<UiLine> lines, ConsoleColor titleColor = ConsoleColor.White)
    {
        int inner = Width - 2;
        int textWidth = inner - 2;

        WriteLine("┌" + new string('─', inner) + "┐", Border);
        BoxLine(title, titleColor, textWidth);
        WriteLine("├" + new string('─', inner) + "┤", Border);

        foreach (UiLine line in lines)
            foreach (string part in Wrap(line.Text, textWidth))
                BoxLine(part, line.Color, textWidth);

        WriteLine("└" + new string('─', inner) + "┘", Border);
    }

    public static string Bar(double percent, int width)
    {
        int filled = (int)Math.Round(Math.Clamp(percent, 0, 100) / 100.0 * width);
        return new string('█', filled) + new string('░', width - filled);
    }

    public void Success(string message) => WriteLine("  [OK] " + message, ConsoleColor.Green);
    public void Error(string message) => WriteLine("  [X]  " + message, ConsoleColor.Red);
    public void Info(string message) => WriteLine("  [i]  " + message, ConsoleColor.Cyan);

    public string? Prompt(string label)
    {
        Console.WriteLine();
        Write("  > " + label + ": ", ConsoleColor.Yellow);
        return Console.ReadLine();
    }

    public void Pause()
    {
        Console.WriteLine();
        Write("  Press Enter to return to the menu...", ConsoleColor.DarkGray);
        Console.ReadLine();
    }

    private static void CardBorderRow(int count, string left, string right, int inner)
    {
        for (int i = 0; i < count; i++)
        {
            Write(left + new string('─', inner) + right, Border);
            if (i < count - 1) Console.Write(" ");
        }
        Console.WriteLine();
    }

    private static void CardTextRow(
        IReadOnlyList<StatCard> cards, int cardWidth,
        Func<StatCard, string> text, Func<StatCard, ConsoleColor> color)
    {
        int textWidth = cardWidth - 4;
        for (int i = 0; i < cards.Count; i++)
        {
            Write("│", Border);
            Write(" " + Fit(text(cards[i]), textWidth) + " ", color(cards[i]));
            Write("│", Border);
            if (i < cards.Count - 1) Console.Write(" ");
        }
        Console.WriteLine();
    }

    private static void BoxLine(string text, ConsoleColor color, int textWidth)
    {
        Write("│", Border);
        Write(" " + Fit(text, textWidth) + " ", color);
        WriteLine("│", Border);
    }

    private static string Fit(string text, int width)
    {
        if (text.Length > width)
            return width > 3 ? text[..(width - 3)] + "..." : text[..width];
        return text.PadRight(width);
    }

    private static IEnumerable<string> Wrap(string text, int width)
    {
        if (text.Length <= width)
        {
            yield return text;
            yield break;
        }

        int indent = Math.Min(text.Length - text.TrimStart().Length, width / 2);
        string pad = new(' ', indent);
        string current = pad;
        bool hasWord = false;

        foreach (string word in text.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (hasWord && current.Length + 1 + word.Length > width)
            {
                yield return current;
                current = pad + word;
            }
            else
            {
                current = hasWord ? current + " " + word : pad + word;
            }
            hasWord = true;
        }

        if (hasWord) yield return current;
    }

    private static void Write(string text, ConsoleColor color)
    {
        ConsoleColor old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = old;
    }

    private static void WriteLine(string text, ConsoleColor color)
    {
        Write(text, color);
        Console.WriteLine();
    }
}