using System;

namespace TR2.PR1ThreadsAndTasks_Laia_Asteroides 
{
    public class Program
    {
        //Tama;o de la pantalla
        public const int desiredHeight = 21, desiredWidth = 40;

        public static readonly object objLock = new();
        public static readonly Random rn = new Random();
        //Creamos la nave
        public static SpaceObject ship = new SpaceObject(desiredWidth/2,desiredHeight);
        //Creamos la lista de los asteroides
        public static List<Asteroid> asteroids = new List<Asteroid>();
        //Estadisticas
        public static int dodged = 0;

        public static async Task Main()
        {
            //Configuramos la pantalla
            Console.CursorVisible = false;
            

            Console.SetWindowSize(20, 10); // tamaño seguro temporal para que no pete
            Console.SetBufferSize(desiredWidth, desiredHeight);
            Console.SetWindowSize(desiredWidth, desiredHeight);

            //Creamos token de cancelacion
            CancellationTokenSource cts = new();
            //Inciamos tareas
            Task webTask = Task.Run(() => WebAnalysisSimulator(cts, cts.Token));
            Task logicTask = Task.Run(() => Logic(cts, cts.Token));
            Task drawingTask = Task.Run(() => Drawing(cts, cts.Token));
            

            try
            {
                await Task.WhenAll(drawingTask, logicTask, webTask);
            }
            catch (OperationCanceledException)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Fin del juego!");
            }

        }

        public static async Task Drawing(CancellationTokenSource cts, CancellationToken token)
        {
            while(true)
            {
                token.ThrowIfCancellationRequested();
                lock (objLock)
                {
                    Console.Clear();

                    foreach (var a in asteroids)
                    {
                        if (a.PosY >= 0 && a.PosY < desiredHeight)
                        {
                            Console.SetCursorPosition(a.PosX, a.PosY);
                            Console.Write("*");
                        }
                    }

                    //Dibujamos la nave
                    Console.SetCursorPosition(ship.PosX, ship.PosY);
                    Console.Write("^");
                }

                await Task.Delay(50);// 20Hz
            }
            
        }
        public static async Task Logic(CancellationTokenSource cts, CancellationToken token)
        {
            
            while (true)
            {
                token.ThrowIfCancellationRequested();
                //NAVE
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
                            cts.Cancel();
                            break;
                        }
                    }
                }
                //ASTEROIDES
                lock (objLock)
                {
                    // Moure asteroides
                    for (int i = 0; i < asteroids.Count; i++)
                    {
                        asteroids[i].PosY++;
                    }

                    // Esborrar els que han sortit de la pantalla
                    asteroids.RemoveAll(a => a.PosY >= desiredHeight);

                    // Comprovar col·lisió
                    foreach (var a in asteroids)
                    {
                        if (a.IsFinalTouched && a.PosX == ship.PosX)
                        {
                            cts.Cancel();
                        }
                    }

                    // Afegir nous asteroides aleatoris
                    if (rn.Next() < 0.2)
                    {
                        asteroids.Add(new Asteroid(rn.Next(0, desiredWidth), 0));
                    }

                    // Comptar esquivats
                    dodged++;
                }

                await Task.Delay(20); // 50Hz
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
    }
    
}

