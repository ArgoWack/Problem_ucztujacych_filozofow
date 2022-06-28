using System;
using System.Threading;
using System.Collections.Generic;
using static System.Console;
namespace Wielowątkowe_Projekt2
{
    // Description:
    
    // Assumptions:
    
    // I assumed k=5 as in standard dilema, however program would work with any given "k".
    // By analogy, I took the same number of seats, forks, books and philosophers as in the problem.
    /*
         How it works; For a philosopher to be able to read a book while eating, it means that he must first choose it (before he sits down or begins to eat).
         Therefore, each philosopher chooses books first, then a place (because to eat you have to sit first). And in the end they eat.
         Each of the philosophers is a separate thread inside which there are 3 locks regarding these 3 sections (book selection, place selection, food).
    */

    class Program
    {
        class filozofowie
        {
            // for locks
            Object Wybór_Ksiazki = new Object();
            Object Wybór_Miejsca = new Object();
            Object Wybór_sztućca = new Object();

            // The lists contain the numbers of individual places, books, forks
            List<int> Ksiazki = new List<int>() { 1, 2, 3, 4, 5 };   // Is the book free? (at the beginning they are all free)
            List<int> Miejsca = new List<int>() { 1, 2, 3, 4, 5 };   // Is the seat free? (at the beginning they are all free)
            List<int> Widelce = new List<int>() { 1, 2, 3, 4, 5 };   // Is the fork free? (at the beginning they are all free)

            [ThreadStatic]
            static int threadSpecificData;

            public void Filozof()
            {
                threadSpecificData = Thread.CurrentThread.ManagedThreadId-4;

                lock (Wybór_Ksiazki)
                {
                    Random seed1 = new Random();
                    //wybór książki
                    int pozycja_ksiazki_w_zbiorze = seed1.Next(0, Ksiazki.Count); // Randomize the book index from the list
                    int ksiazka_wybrana = Ksiazki[pozycja_ksiazki_w_zbiorze]; // elects a book from the given index

                    //removes a book from the list
                    Ksiazki.RemoveAt(pozycja_ksiazki_w_zbiorze); //Deletes the book from the given index (because it is taken)

                    WriteLine("Filozof "+ threadSpecificData +" wybrał książkę: " + (ksiazka_wybrana));
                }

                int miejsce_wybrane;
                lock (Wybór_Miejsca)
                {
                    Random seed2 = new Random();
                    //choice of seat
                    int pozycja_miejsca_w_zbiorze = seed2.Next(0, Miejsca.Count);
                    miejsce_wybrane = Miejsca[pozycja_miejsca_w_zbiorze];

                    //removes a seat from the list
                    Miejsca.RemoveAt(pozycja_miejsca_w_zbiorze);

                    WriteLine("Filozof " + threadSpecificData + " wybrał miejsce: " + (miejsce_wybrane));
                }
                    // Forks
                    bool czy_głodny = true;
                    while (czy_głodny)
                    { 

                        lock (Wybór_sztućca)
                        { 
                            if (miejsce_wybrane == 1)
                            {
                                if (Widelce.Contains(5) && Widelce.Contains(1))
                                {
                                    Widelce.Remove(5);
                                    Widelce.Remove(1);

                                    WriteLine("Filozof " + threadSpecificData + " na miejscu: " + (miejsce_wybrane) + " zaczął jeść");

                                    Thread.Sleep(10);

                                    WriteLine("Filozof " + threadSpecificData + " na miejscu: " + (miejsce_wybrane) + " zjadł");

                                    Widelce.Add(5);
                                    Widelce.Add(1);

                                    czy_głodny = false;
                                }
                            }
                            else
                            {
                                if (Widelce.Contains(miejsce_wybrane - 1) && Widelce.Contains(miejsce_wybrane))
                                {
                                    Widelce.Remove(miejsce_wybrane-1);
                                    Widelce.Remove(miejsce_wybrane);

                                    WriteLine("Filozof " + threadSpecificData + " na miejscu: " + (miejsce_wybrane) + " zaczął jeść");

                                    Thread.Sleep(10);


                                    WriteLine("Filozof " + threadSpecificData + " na miejscu: " + (miejsce_wybrane) + " zjadł");

                                    Widelce.Add(miejsce_wybrane-1);
                                    Widelce.Add(miejsce_wybrane);

                                    czy_głodny = false;
                                }
                            }
                        }
                        Random seed4 = new Random();
                        Thread.Sleep(seed4.Next(0, 2) * 10);
                        // If one or both are occupied, wait (or after finished meal rests)
                    }
                
            }

        }
        static void Main(string[] args)
        {
            filozofowie akcje = new filozofowie();

            // There are 5 philosophers, each one is a separate thread
            Thread Filozof_1 = new Thread(akcje.Filozof);
            Thread Filozof_2 = new Thread(akcje.Filozof);
            Thread Filozof_3 = new Thread(akcje.Filozof);
            Thread Filozof_4 = new Thread(akcje.Filozof);
            Thread Filozof_5 = new Thread(akcje.Filozof);

            Filozof_1.Start(); Filozof_2.Start(); Filozof_3.Start(); Filozof_4.Start(); Filozof_5.Start();

            Filozof_1.Join(); Filozof_2.Join(); Filozof_3.Join(); Filozof_4.Join(); Filozof_5.Join();

            WriteLine("Wszyscy zjedli");
            ReadKey();
        }
    }
}
// variables: Number of philosophers, k - number of books, number of forks, number of seats

/*
The dining philosophers problem:
They may sit at a different place each time. They always take the forks from the vicinity of the chosen place setting, but they also choose a place randomly.
*/


/*
choosing a book-> choosing a place -> taking 2 forks (from neighboring covers)


For a single philosopher:


-> book selection (Out of free draw)

-> seat selection (Out of the free draw)

-> Trying to eat (Check if both adjacent forks are free)

If so, then eat it and lock the forks until it's done.

If not, it waits until both are free.

After eating, he puts his forks down.

*/

//Copyright: Hubert Kieslich
