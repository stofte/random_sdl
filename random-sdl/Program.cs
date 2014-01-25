using SdlDotNet.Graphics;
using SdlDotNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SdlDotNet.Audio;

namespace Board
{
    class Flash
    {
        public int Tick;
        public Surface Surface;
        public Point Point;
    }

    class Program
    {
        Surface background;
        Surface screen;
        ulong tick;
        Random random;

        List<Flash> flashes;

        Func<Point> next_point;
        Func<int> next_gen;
        Func<Color> next_color;
        Func<Surface> next_surface;

        int next;

        int width = 550;
        int height = 650;

        public Program()
        {
            next_gen = () => 100 + random.Next(100); // ~150 ish frames

            next_color = () =>
            {
                KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
                KnownColor randomColorName = names[random.Next(names.Length)];
                return Color.FromKnownColor(randomColorName);
            };

            next_point = () => new Point
            {
                X = random.Next(10, width - 60),
                Y = random.Next(10, height - 60)
            };

            next_surface = () => new Surface(new Size
            {
                Height = random.Next(100, 200),
                Width = random.Next(100, 200)
            });

            flashes = new List<Flash>();
            random = new Random();
            screen = Video.SetVideoMode(width, height);
            background = new Surface("resources/image.jpeg").Convert(screen);


            next = next_gen();

            Events.Tick += Events_Tick;
            Events.Run();
        }

        void Events_Tick(object sender, TickEventArgs e)
        {
            // randomly start animations
            if (--next == 0)
            {
                var flash = new Flash { Surface = next_surface(), Point = next_point(), Tick = random.Next(100, 1000) };
                flash.Surface.Alpha = (byte)random.Next(100, 255);
                flash.Surface.AlphaBlending = true;
                flash.Surface.Fill(next_color());

                flashes.Add(flash);
                next = next_gen();
            }

            screen.Blit(background);

            var keep = new List<Flash>();
            foreach (var flash in flashes)
            {
                if (--flash.Tick > 0)
                {
                    keep.Add(flash);
                }
                else
                {
                    Sound snd = new Sound("resources/cow4.wav");
                    snd.Volume = 40;
                    snd.Play();
                }
                screen.Blit(flash.Surface, flash.Point);
                flash.Tick--;
            }

            flashes = keep;

            screen.Update();
            tick++;
            //Console.WriteLine(tick);
        }



        static void Main(string[] args)
        {
            new Program();
        }
    }
}
