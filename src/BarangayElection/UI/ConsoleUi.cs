namespace BarangayElection.UI;

/// <summary>One line of text inside a panel, with its colour.</summary>
internal readonly record struct UiLine(string Text, ConsoleColor Color = ConsoleColor.Gray);

/// <summary>A small "stat card" like the ones at the top of a dashboard.</summary>
internal readonly record struct StatCard(string Label, string Value, string Note, ConsoleColor Color);

/// <summary>
/// All console drawing lives here (cards, panels, bars, prompts), so the rest of the
/// program never touches Console colours or box characters. To change the look of the
/// app, edit this file only.
/// </summary>
internal sealed class ConsoleUi
{
    public const int Width = 79;
    private const ConsoleColor Border = ConsoleColor.DarkGray;

    // ---------- Screen helpers ----------

    public void Clear()
    {
        try { Console.Clear(); }
        catch (IOException) { /* output is redirected (e.g. during tests) - ignore */ }
    }

    public void Header(string title, string subtitle)
    {
        Console.WriteLine();
        WriteLine("  " + title, ConsoleColor.Cyan);
        WriteLine("  " + subtitle, ConsoleColor.DarkGray);
        Console.WriteLine();
    }

    public void BlankLine() => Console.WriteLine();

    // ---------- Dashboard components ----------

    /// <summary>Row of stat cards (label, big value, small note).</summary>
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

    /// <summary>A bordered panel with a title and any number of lines (long lines are wrapped).</summary>
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

    /// <summary>Horizontal bar such as ███████░░░░░ for a percentage.</summary>
    public static string Bar(double percent, int width)
    {
        int filled = (int)Math.Round(Math.Clamp(percent, 0, 100) / 100.0 * width);
        return new string('█', filled) + new string('░', width - filled);
    }

    // ---------- Messages & input ----------

    public void Success(string message) => WriteLine("  [OK] " + message, ConsoleColor.Green);
    public void Error(string message) => WriteLine("  [X]  " + message, ConsoleColor.Red);
    public void Info(string message) => WriteLine("  [i]  " + message, ConsoleColor.Cyan);

    /// <summary>Asks for text. Returns null if input has ended (Ctrl+Z / Ctrl+D).</summary>
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

    // ---------- Private drawing helpers ----------

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
        int textWidth = cardWidth - 4; // minus 2 borders and 2 padding spaces
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

    /// <summary>Pads or trims text to exactly <paramref name="width"/> characters.</summary>
    private static string Fit(string text, int width)
    {
        if (text.Length > width)
            return width > 3 ? text[..(width - 3)] + "..." : text[..width];
        return text.PadRight(width);
    }

    /// <summary>Word-wraps text, keeping its leading indentation on continuation lines.</summary>
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
