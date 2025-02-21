using System.Runtime.Versioning;

namespace Unit_X_Common
{
    [SupportedOSPlatform("windows")]
    public class Selector<TServer, TClient>
        where TServer : ServerRuntime, new()
        where TClient : ClientRuntime, new()
    {
        private readonly string _header;
        private readonly string _description;
        private readonly int _redrawCursorRow;

        private void OnSelectServer() => new TServer().Start();
        private void OnSelectClient() => new TClient().Start();

        private readonly struct Selectable
        {
            public required string Option { get; init; }
            public required Action OnConfirmSelected { get; init; }
        }

        private readonly List<Selectable> _selectables;
        private int _selected;

        private void DrawSelection()
        {
            for (int i = 0; i < _selectables.Count; i++)
            {
                if (i == _selected)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine($"> {_selectables[i].Option}");
                    Console.ResetColor();
                }
                else
                    Console.WriteLine($"* {_selectables[i].Option}");
            }
        }
        private void Draw()
        {
            Console.WriteLine($"-> {_header} <-");
            Console.WriteLine(_description);

            DrawSelection();
        }
        private void Redraw()
        {
            Console.SetCursorPosition(0, _redrawCursorRow);
            DrawSelection();
        }

        public Selector(int unit, IEnumerable<string> description)
        {
            _header = $"Welcome to unit test {unit}";
            _description = string.Join('\n', description);
            _redrawCursorRow = description.Count() + 1;

            _selectables = [
                new Selectable()
                {
                    Option = "Start server",
                    OnConfirmSelected = OnSelectServer
                },
                new Selectable()
                {
                    Option = "Start client",
                    OnConfirmSelected = OnSelectClient
                }];
            _selected = 0;
        }

        public void Start()
        {
            Draw();

            bool isRunning = true;
            while (isRunning)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        _selected = (_selected - 1 + _selectables.Count) % _selectables.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        _selected = (_selected + 1) % _selectables.Count;
                        break;
                    case ConsoleKey.Enter:
                        isRunning = false;
                        break;
                    case ConsoleKey.Escape:
                        return;
                }

                Redraw();
            }

            Console.Clear();

            _selectables[_selected].OnConfirmSelected();
        }
    }
}
