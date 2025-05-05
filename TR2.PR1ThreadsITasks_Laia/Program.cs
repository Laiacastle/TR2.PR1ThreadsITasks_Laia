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
        static void Main()
        {
            //Crear comersals
            Thread Comensal1 = new Thread(() => Menjar(1));
            Thread Comensal2 = new Thread(() => Menjar(2));
            Thread Comensal3 = new Thread(() => Menjar(3));
            Thread Comensal4 = new Thread(() => Menjar(4));
            Thread Comensal5 = new Thread(() => Menjar(5));
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
                Console.WriteLine(MsgPensar, id);
                Thread.Sleep(pensar);
                

                if (paletsC[0].Agafat || paletsC[1].Agafat)
                {
                     continue; // reintentar en el bucle
                }
                Console.WriteLine(MsgAgafarPaletEsq, id);
                paletsC[0].Agafat = true;
                Console.WriteLine(MsgAgafarPaletDret, id);
                paletsC[1].Agafat = true;

                Console.WriteLine(MsgMenjar, id);
                Thread.Sleep(menjar);
                
                Console.WriteLine(MsgDeixarPaletEsq, id);
                
                paletsC[0].Agafat = false;
                Console.WriteLine(MsgDeixarPaletDret, id);
                paletsC[1].Agafat = false;
                

            }
        }
    }
}
