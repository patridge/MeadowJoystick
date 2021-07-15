using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Sensors.Hid;
//using Meadow.Foundation.Sensors.Hid;
//using Meadow.Peripherals.Sensors.Hid;
using Meadow2.Foundation.Sensors.Hid;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        Max7219 display;
        GraphicsLibrary graphics;
        AnalogJoystick joystick;

        public MeadowApp()
        {
            Console.WriteLine("ctor");
            var init = Initialize();
            Console.WriteLine("Continue after init");
            init.ContinueWith(_ =>
            {
                // Keep from starting gampe loop until everything in Initialize is done.
                StartGameLoop();
            });
        }

        const int maxX = 8;
        const int maxY = 36;
        Point currentLocation = new Point(0, 0);

        Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            display = new Max7219(
                device: Device,
                spiBus: Device.CreateSpiBus(),
                csPin: Device.Pins.D01,
                deviceCount: 4,
                maxMode: Max7219.Max7219Type.Display);

            graphics = new GraphicsLibrary(display);
            graphics.CurrentFont = new Font4x8();
            graphics.Rotation = GraphicsLibrary.RotationType._180Degrees;
            //joystick = new AnalogJoystick(Device, Device.Pins.A00, Device.Pins.A01, null, true);
            joystick = new AnalogJoystick(
                Device.CreateAnalogInputPort(Device.Pins.A00, 1, 10),
                Device.CreateAnalogInputPort(Device.Pins.A01, 1, 10),
                null, false);
            var centerJoystick = joystick.SetCenterPosition();
            var configureUpdates = centerJoystick.ContinueWith(_ =>
            {
                // Trying not to tie in to the sensor data until centering is completed, since fire-and-forget seems unsafe there.
                Console.WriteLine("Done centering...");
                joystick.Updated += Joystick_Updated;
                joystick.StartUpdating(TimeSpan.FromMilliseconds(20));
                Console.WriteLine("Done subscribing...");
            });
            Console.WriteLine("Done initialize...");
            return configureUpdates;
        }

        private void Joystick_Updated(object sender, IChangeResult<JoystickPosition> e)
        {
            // This is currently able to log if I do `joystick.Updated` and `joystick.StartUpdating`.
            //Console.WriteLine($"pos: {e.New.Horizontal}, {e.New.Vertical}");

            // Set currentLocation according to joystick, clamped by maxX,maxY

            //var pos = await joystick.Position.GetPosition(); // Old...doesn't work anymore.
            //var pos = await joystick.Read(); // Returns a JoystickPosition, which doesn't work with DigitalJoystickPosition.
            //Console.WriteLine($"pos: {e.New.Horizontal}, {e.New.Vertical}");
            //Console.WriteLine($"pos: {pos.Horizontal}, {pos.Vertical}");
            //var pos = joystick.DigitalPosition;
            //if (pos == AnalogJoystick.DigitalJoystickPosition.Left)
            //if (e.New.Horizontal < 0.2f) //if (pos == DigitalJoystickPosition.Left)
            //{
            //    Console.WriteLine("Left");
            //    currentLocation.X -= 1;
            //}
            //if (e.New.Vertical > 0f) //if (pos == DigitalJoystickPosition.Right)
            //{
            //    Console.WriteLine("Right");
            //    currentLocation.X += 1;
            //}
            //if (e.New.Horizontal > 0) //if (pos == DigitalJoystickPosition.Up)
            //{
            //    Console.WriteLine("Up");
            //    currentLocation.Y += 1;
            //}
            //if (pos == DigitalJoystickPosition.Down)
            //{
            //    Console.WriteLine("Down");
            //    currentLocation.Y -= 1;
            //}

            //currentLocation.X = Math.Clamp(currentLocation.X, 0, maxX);
            //currentLocation.Y = Math.Clamp(currentLocation.Y, 0, maxY);
            //if (e.New.Horizontal < 0.2f)
            //{
            //    Left.SetBrightness(0f);
            //    Right.SetBrightness(0f);
            //}
            //if (e.New.Vertical < 0.2f)
            //{
            //    Up.SetBrightness(0f);
            //    Down.SetBrightness(0f);
            //}

            //if (e.New.Horizontal > 0)
            //    Left.SetBrightness(e.New.Horizontal.Value);
            //else
            //    Right.SetBrightness(Math.Abs(e.New.Horizontal.Value));

            //if (e.New.Vertical > 0)
            //    Down.SetBrightness(Math.Abs(e.New.Vertical.Value));
            //else
            //    Up.SetBrightness(Math.Abs(e.New.Vertical.Value));

            //Console.WriteLine($"({e.New.Horizontal.Value}, {e.New.Vertical.Value})");
        }

        int tick = 0;
        void StartGameLoop()
        {
            Console.WriteLine("Game loop starting");
            while (true)
            {
                tick++;
                CheckInput(tick);

                graphics.Clear();
                DrawGame();
                graphics.Show();

                Thread.Sleep(50);
            }
        }

        void CheckInput(int tick)
        {
            // Set currentLocation according to joystick, clamped by maxX,maxY

            //var pos = await joystick.Position.GetPosition(); // Old...doesn't work anymore.
            //var pos = await joystick.Read(); // Returns a JoystickPosition, which doesn't work with DigitalJoystickPosition.
            //Console.WriteLine($"pos: {pos.Horizontal}, {pos.Vertical}");
            // TODO: Figure out how to get calibration done, since `TranslateAnalogPositionToDigital` uses that data under the hood (and it is wrong by default for my analog joystick).
            var pos = joystick.DigitalPosition;
            Console.WriteLine($"pos: {pos}");
            ////if (pos == AnalogJoystick.DigitalJoystickPosition.Left)
            //if (pos == DigitalJoystickPosition.Left)
            //{
            //    Console.WriteLine("Left");
            //    currentLocation.X -= 1;
            //}
            //if (pos == DigitalJoystickPosition.Right)
            //{
            //    Console.WriteLine("Right");
            //    currentLocation.X += 1;
            //}
            //if (pos == DigitalJoystickPosition.Up)
            //{
            //    Console.WriteLine("Up");
            //    currentLocation.Y += 1;
            //}
            //if (pos == DigitalJoystickPosition.Down)
            //{
            //    Console.WriteLine("Down");
            //    currentLocation.Y -= 1;
            //}

            //currentLocation.X = Math.Clamp(currentLocation.X, 0, maxX);
            //currentLocation.Y = Math.Clamp(currentLocation.Y, 0, maxY);
        }

        void DrawGame()
        {
            graphics.DrawPixel(currentLocation.X, currentLocation.Y);
        }
    }
}
