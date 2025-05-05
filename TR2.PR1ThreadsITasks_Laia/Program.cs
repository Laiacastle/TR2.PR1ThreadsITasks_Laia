namespace TR2.PR1ThreadsITasks_Laia
{
    public class Program
    {
        //Crear els palets
        public static Palet paletAEsq = new Palet(1, false);
        public static Palet paletBDret = new Palet(2, false);
        public static Palet paletCEsq = new Palet(3, false);
        public static Palet paletDDret = new Palet(4, false);
        public static Palet paletE = new Palet(5, false);
        public static Palet[] palets = [paletAEsq, paletBDret, paletCEsq, paletDDret, paletE];

        //Crear Cola
        public static Queue<Thread> Cola = new Queue<Thread>();

        //Crear locks
        public static object consolaLock = new object();
        public static object paletLock = new object();
        static void Main()
        {
            //Crear comersals
            Thread Comensal1 = new Thread(() => Menjar(1));
            Thread Comensal2 = new Thread(() => Menjar(2));
            Thread Comensal3 = new Thread(() => Menjar(3));
            Thread Comensal4 = new Thread(() => Menjar(4));
            Thread Comensal5 = new Thread(() => Menjar(5));

            //Afegir Comersals a la cola
            Cola.Enqueue(Comensal1);
            Cola.Enqueue(Comensal2);
            Cola.Enqueue(Comensal3);
            Cola.Enqueue(Comensal4);
            Cola.Enqueue(Comensal5);

            //iniciar cola
            while (Cola.Count > 0)
            {
                Thread t = Cola.Dequeue();
                t.Start();
            }
        }

        public static void Menjar(int id)
        {
            while (true)
            {
                const string MsgPensar = "l'Comensal {0} esta pensant que vol per menjar...",
                    MsgMenjar = "l'Comensal {0} esta manjant...",
                    MsgAgafarPaletEsq = "l'Comensal {0} ha agafat el palet esquerra",
                    MsgAgafarPaletDret = "l'Comensal {0} ha agafat el palet dret",
                    MsgDeixarPaletEsq = "l'Comensal {0} ha deixat el palet esquerra",
                    MsgDeixarPaletDret = "l'Comensal {0} ha deixat el palet dret";

                string color = "Black";
                Random r = new Random();
                int pensar = r.Next(500, 2000);
                int menjar = r.Next(500, 1000);
                Palet[] paletsC = { palets.Where(n => n.Id == id).FirstOrDefault(), palets.Where(n => n.Id == id - 1).FirstOrDefault() };
                DateTime iniciFam = DateTime.Now;
                //Canvia el color de consola depenen del comensal, en cas de que sigui el 1 li asigna el palet dret 5
                switch (id)
                {
                    case 1:
                        color = "Green";
                        paletsC[1] = palets.Where(n => n.Id == 5).FirstOrDefault();
                        break;
                    case 2: color = "Red"; break;
                    case 3: color = "Yellow"; break;
                    case 4: color = "Blue"; break;
                    case 5: color = "Magenta"; break;
                }
                //Comensal pensa
                ChangeTextColor(color, MsgPensar, id);
                Thread.Sleep(pensar);
                //Comprova que no estiguin agafats els palets i bloqueja els palets mentres esta cambiant el seu estat
                lock (paletLock)
                {
                    if (paletsC[0].Agafat || paletsC[1].Agafat)
                    {
                        continue; // reintentar en el bucle
                    }
                    //Agafa el palet esquerre
                    ChangeTextColor(color, MsgAgafarPaletEsq, id);
                    paletsC[0].Agafat = true;
                    //Agafa el palet dret
                    ChangeTextColor(color, MsgAgafarPaletDret, id);
                    paletsC[1].Agafat = true;

                }
                //Menja
                ChangeTextColor(color, MsgMenjar, id);
                Thread.Sleep(menjar);
                //Deixa el palet esquerre
                ChangeTextColor(color, MsgDeixarPaletEsq, id);
                lock (paletLock)
                {
                    paletsC[0].Agafat = false;
                    //Deixa el palet dret
                    ChangeTextColor(color, MsgDeixarPaletDret, id);
                    paletsC[1].Agafat = false;
                }
            }
        }

        public static void ChangeTextColor(string color, string msg, int id)
        {

            lock (consolaLock)
            {
                switch (color)
                {
                    case "Magenta": Console.ForegroundColor = ConsoleColor.Magenta; break;
                    case "Blue": Console.ForegroundColor = ConsoleColor.Blue; break;
                    case "Green": Console.ForegroundColor = ConsoleColor.Green; break;
                    case "Yellow": Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case "Red": Console.ForegroundColor = ConsoleColor.Red; break;
                    default: Console.ForegroundColor = ConsoleColor.Black; break;
                }
                Console.WriteLine(msg, id);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

        }
    }
}
