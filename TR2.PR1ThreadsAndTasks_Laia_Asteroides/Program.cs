using System;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace TR2.PR1ThreadsAndTasks_Laia_Asteroides
{
    public class Program
    {
        public const int desiredHeight = 20, desiredWidth = 40;

        public static readonly object objLock = new();
        public static readonly Random rn = new Random();
        public static Ship ship = new Ship(desiredWidth / 2, desiredHeight - 1);
        public static List<Asteroid> asteroids = new List<Asteroid>();
        public static int dodged = 0;
        public static int game = 0;
        public static bool isRunning = true;

        public static async Task Main()
        {
            const string MsgEnd = "Fin del juego!",
                         MsgSeconds = "Asteroides esquivados: {0}",
                         MsgTimePlayed = "Tiempo de juego: {0} segundos",
                         Menu = "--------------\n" +
                                 "| 1   Play   |\n" +
                                 "--------------\n" +
                                 "| 2   Stats  |\n" +
                                 "--------------\n" +
                                 "| 3   Exit   |\n" +
                                 "--------------\n";

            Console.CursorVisible = false;
            Console.SetWindowSize(20, 10);
            Console.SetBufferSize(desiredWidth + 1, desiredHeight);
            Console.SetWindowSize(desiredWidth + 1, desiredHeight);

            while (isRunning)
            {
                var statsList = GetStats();
                game = (statsList.Count > 0) ? statsList.Max(s => s.Game) : 0;
                var selection = await MenuT(Menu);
                switch (selection)
                {
                    case 1:
                        game++;
                        ship = new Ship(desiredWidth / 2, desiredHeight - 1);
                        ship.Lives = 3;
                        dodged = 0;

                        var sw = Stopwatch.StartNew();

                        while (ship.Lives > 0)
                        {
                            CancellationTokenSource cts = new();
                            asteroids.Clear();

                            Task webTask = Task.Run(() => WebAnalysisSimulator(cts, cts.Token));
                            Task logicTask = Task.Run(() => Logic(cts, cts.Token));
                            Task drawingTask = Task.Run(() => Drawing(cts, cts.Token));

                            try
                            {
                                await Task.WhenAny(drawingTask, logicTask, webTask);
                                if (drawingTask.IsFaulted) await drawingTask;
                                if (logicTask.IsFaulted) await logicTask;
                                if (webTask.IsFaulted) await webTask;

                                if (drawingTask.IsCanceled || logicTask.IsCanceled || webTask.IsCanceled)
                                    throw new OperationCanceledException();
                            }
                            catch (OperationCanceledException)
                            {
                                ship.Lives--;

                                if (ship.Lives <= 0)
                                {
                                    sw.Stop();
                                    Console.Clear();
                                    WriteCentered(MsgEnd, 5);
                                    WriteCentered(string.Format(MsgSeconds, dodged), 7);
                                    WriteCentered(string.Format(MsgTimePlayed, sw.ElapsedMilliseconds / 1000), 8);
                                    await InsertIntoFile(new Stats(game, sw.ElapsedMilliseconds / 1000, dodged));
                                    WriteCentered("Pulsa cualquier tecla para continuar...", 10);
                                    Console.ReadKey(true);
                                }
                            }
                        }
                        break;

                    case 2:
                        await ViewStats();
                        break;

                    case 3:
                        isRunning = false;
                        break;
                }
            }
        }

        public static async Task<int> MenuT(string menu)
        {
            Console.CursorVisible = true;
            int choice = 0;
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                string[] lines = menu.Split('\n');
                int topOffset = (Console.WindowHeight - lines.Length) / 2;

                foreach (var line in lines)
                {
                    WriteCentered(line, topOffset++);
                }

                Console.ResetColor();
                Console.SetCursorPosition((Console.WindowWidth - 20) / 2, topOffset + 1);
                string input = Console.ReadLine() ?? "";

                if (input == "1" || input == "2" || input == "3")
                {
                    choice = int.Parse(input);
                    break;
                }
            }
            Console.CursorVisible = false;
            return choice;
        }

        public static void WriteCentered(string text, int topOffset)
        {
            int left = Math.Max((Console.WindowWidth - text.Length) / 2, 0);
            Console.SetCursorPosition(left, topOffset);
            Console.WriteLine(text);
        }

        public static async Task Drawing(CancellationTokenSource cts, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                lock (objLock)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(0, 0);
                    for (int i = 0; i < ship.Lives; i++)
                    {
                        Console.Write("[♥]");
                    }

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.SetCursorPosition(20, 0);
                    Console.Write("Pulsa 'Q' para salir");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    foreach (var a in asteroids)
                    {
                        if (a.PosY >= 0 && a.PosY < desiredHeight)
                        {
                            Console.SetCursorPosition(a.PosX, a.PosY);
                            Console.Write("*");
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.SetCursorPosition(ship.PosX, ship.PosY);
                    Console.Write("^");

                    Console.ResetColor();
                }

                await Task.Delay(50);
            }
        }

        public static async Task Logic(CancellationTokenSource cts, CancellationToken token)
        {
            int asteroidSpawnInterval = 500;
            DateTime lastAsteroidSpawn = DateTime.Now;

            while (true)
            {
                token.ThrowIfCancellationRequested();

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    lock (objLock)
                    {
                        if (key == ConsoleKey.A && ship.PosX > 0)
                            ship.PosX--;
                        else if (key == ConsoleKey.D && ship.PosX < desiredWidth - 1)
                            ship.PosX++;
                        else if (key == ConsoleKey.Q)
                        {
                            ship.Lives = 0;
                            cts.Cancel();
                            token.ThrowIfCancellationRequested();
                        }
                    }
                }

                lock (objLock)
                {
                    for (int i = 0; i < asteroids.Count; i++)
                    {
                        asteroids[i].PosY++;
                    }

                    var removed = asteroids.RemoveAll(a => a.PosY >= desiredHeight);
                    dodged += removed;

                    foreach (var a in asteroids)
                    {
                        if (a.PosY == ship.PosY && a.PosX == ship.PosX)
                        {
                            Console.Beep(); // Sonido de colisión
                            cts.Cancel();
                            token.ThrowIfCancellationRequested();
                        }
                    }

                    if ((DateTime.Now - lastAsteroidSpawn).TotalMilliseconds >= asteroidSpawnInterval)
                    {
                        asteroids.Add(new Asteroid(rn.Next(1, desiredWidth), 0));
                        lastAsteroidSpawn = DateTime.Now;
                    }
                }

                await Task.Delay(20);
            }
        }

        public static async Task WebAnalysisSimulator(CancellationTokenSource cts, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(30000);
                cts.Cancel();
            }
        }

        public static async Task<List<Stats>> ViewStats()
        {
            List<Stats> stats = new List<Stats>();
            string filePath = "../../../estadisticas.csv";

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;

            if (!File.Exists(filePath))
            {
                WriteCentered("Archivo de estadísticas no encontrado.", 5);
                Console.ResetColor();
                Console.WriteLine("\nPulsa cualquier tecla para volver...");
                Console.ReadKey(true);
                Console.Clear();
                return stats;
            }

            using (var reader = new StreamReader(filePath))
            {
                string? line;
                bool isFirstLine = true;

                WriteCentered("=== Estadísticas de Partidas ===", 1);
                Console.WriteLine();

                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    var parts = line.Split(';');
                    if (parts.Length == 3)
                    {
                        stats.Add(new Stats(int.Parse(parts[0]), long.Parse(parts[1]), int.Parse(parts[2])));
                        Console.WriteLine($"Partida #{parts[0]} | Tiempo (s): {parts[1]} | Asteroides esquivados: {parts[2]}");
                    }
                }
            }
            

            Console.ResetColor();
            Console.WriteLine("\nPulsa cualquier tecla para volver al menú...");
            Console.ReadKey(true);
            Console.Clear();
            return stats;
        }

        public static async Task InsertIntoFile(Stats stats)
        {
            string filePath = "../../../estadisticas.csv";
            bool fileExists = File.Exists(filePath);

            using (var writer = new StreamWriter(filePath, append: true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Game;SecondsPlayed;AsteroidDodged");
                }

                writer.WriteLine($"{stats.Game};{stats.SecondsPlayed};{stats.AsteroidDodged}");
            }
        }

        public static List<Stats> GetStats()
        {
            List<Stats> stats = new List<Stats>();
            string filePath = "../../../estadisticas.csv";


            if (!File.Exists(filePath))
            {
                return stats;
            }

            using (var reader = new StreamReader(filePath))
            {
                string? line;
                bool isFirstLine = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    var parts = line.Split(';');
                    if (parts.Length == 3)
                    {
                        stats.Add(new Stats(int.Parse(parts[0]), long.Parse(parts[1]), int.Parse(parts[2])));
                        
                    }
                }
            }
            return stats;
        }
    }
}
