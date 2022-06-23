using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions.Canvas.Model;

namespace CrozierCreativeWA.Services;

public class SnakeGameService : ISnakeGameService
{
    public BECanvasComponent? _canvasReference { get; private set; }
    public Canvas2DContext? _context { get; private set; }
    public SnakeGame? snakeGame { get; set; }

    public long canvasHeight => 400;
    public long canvasWidth => 350;

    public bool isGameActive { get; set; }

    private string title => "SNAKE";
    private string instructions => "Press any arrow button or key to start...";
    private string? currentDirection { get; set; }



    public async Task InitService(BECanvasComponent? canvasReference)
    {
        _canvasReference = canvasReference;

        _context = await _canvasReference.CreateCanvas2DAsync();

        await _context!.SetStrokeStyleAsync("green");
        //set font style first before measuring text.
        await _context.SetFontAsync("48px monospace");

        TextMetrics snakeTextMetrics = await _context.MeasureTextAsync(title);
        double X = (canvasWidth / 2) - (snakeTextMetrics.Width / 2);
        double Y = canvasHeight / 2;
        //render SNAKE title
        //note*only batch drawing effects, otherwise issues can come up with the position of the drawings..
        await _context.BeginBatchAsync();
        await _context.StrokeTextAsync(title, (canvasWidth / 2) - (snakeTextMetrics.Width / 2), canvasHeight / 2);
        await _context.EndBatchAsync();

        //setup instruction text
        await _context.SetFillStyleAsync("green");

        await _context.SetFontAsync("14px monospace");

        TextMetrics instructTextMetrics = await _context.MeasureTextAsync(instructions);

        //render instructions
        await _context.BeginBatchAsync();
        await _context.FillTextAsync(instructions, (canvasWidth / 2) - (instructTextMetrics.Width / 2), (canvasHeight / 2) + 30);
        await _context.EndBatchAsync();
    }

    public async Task StartSnakeGame()
    {
        snakeGame = new SnakeGame(canvasHeight, canvasWidth);
        isGameActive = true;
        await ClearCanvas();
        await SpawnApple();
        await SpawnSnake();
    }

    public async Task ChangeSnakeDirection(string newDirection) =>
        await Task.Run(async () =>
        {
            currentDirection = newDirection;
            await SpawnApple();

        });
    

    private async Task ClearCanvas() => await _context!.ClearRectAsync(0, 0, canvasWidth, canvasHeight);

    private async Task SpawnApple()
    {
        //clear old apple
        await _context!.ClearRectAsync(snakeGame!.Apple!.PosX, snakeGame!.Apple!.PosY, snakeGame!.Apple!.Width, snakeGame!.Apple!.Height);

        snakeGame.RefreshApplePosition();

        await _context!.SetFillStyleAsync(snakeGame!.Apple!.Color);

        await _context!.FillRectAsync(snakeGame!.Apple!.PosX, snakeGame!.Apple!.PosY, snakeGame!.Apple!.Width, snakeGame!.Apple!.Height);
    }

    private async Task SpawnSnake()
    {
        await _context!.SetFillStyleAsync(snakeGame!.Snake!.Color);

        await _context!.FillRectAsync(snakeGame!.Snake!.HeadPosX, snakeGame!.Snake!.HeadPosY, snakeGame!.Snake!.BaseWidth, snakeGame!.Snake!.BaseHeight);
    }


    public class SnakeGame
    {
        public DateTime DatePlayed { get; set; }
        public Snake? Snake { get; set; }
        public Apple? Apple { get; set; }
        public long Score { get; set; }
        private double Height { get; set; }
        private double Width { get; set; }

        public SnakeGame(double height, double width)
        {
            Height = height;
            Width = width;

            DatePlayed = DateTime.Now;
            //should set the starting position of snake head to the center of canvas
            //make sure values are valeus of 10
            double tenthX = Math.Ceiling((Width / 2) / 10) * 10;
            double tenthY = Math.Ceiling((Height / 2) / 10) * 10;

            Snake = new Snake(tenthX, tenthY);
            //the random apple coordinates could result in part of the apple being off canvas, need to consider the 10,10 dimensions of apple and prevent overflow
            RefreshApplePosition();
            
            Score = 0;
        }

        public void RefreshApplePosition()
        {
            if (Apple == null)
                Apple = new Apple();

            double randX = Convert.ToDouble(Random.Shared.NextInt64(0, Convert.ToInt64(Width)));
            double randY = Convert.ToDouble(Random.Shared.NextInt64(0, Convert.ToInt64(Height)));


            Apple.PosX = Math.Ceiling(randX / 10) * 10;
            Apple.PosY = Math.Ceiling(randY / 10) * 10;

        }

    }

    public class Apple
    {
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Color { get; set; }

        public Apple()
        {
            
            this.PosX = 0;
            this.PosY = 0;
            Width = 10;
            Height = 10;
            Color = "red";
        }
    }


    public class Snake
    {
        public double HeadPosX { get; set; }
        public double HeadPosY { get; set; }
        public double BaseWidth { get { return 10; } }
        public double BaseHeight { get { return 10; } }
        public double Length { get; set; }
        public string Color { get; set; }

        public Snake(double HeadPosX, double HeadPosY)
        {
            //reduce x and y by half base width and height to get true center
            this.HeadPosX = HeadPosX - (BaseWidth / 2);
            this.HeadPosY = HeadPosY - (BaseHeight / 2);
            //default Length to 10, 10 being the width of one Snake square, when the Snake grows Length should increment in amounts of 10.
            Length = 10;
            Color = "green";
        }
    }

}
