using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace CrozierCreativeWA.Services;

public interface ISnakeGameService
{
    BECanvasComponent? _canvasReference { get; }
    Canvas2DContext? _context { get; }
    long canvasHeight { get; }
    long canvasWidth { get; }
    SnakeGameService.SnakeGame? snakeGame { get; set; }
    bool isGameActive { get; set; }

    Task InitService(BECanvasComponent? canvasReference);

    Task StartSnakeGame();

    Task ChangeSnakeDirection(string direction);

    Task MoveSnake();
}