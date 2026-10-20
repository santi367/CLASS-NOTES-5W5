using Dessins.Events;
using Microsoft.AspNetCore.Mvc;

namespace Dessins.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DessinsController : ControllerBase
    {
        [HttpGet]
        // Rien à modifier ici, juste un exemple de dessin très simple
        public ActionResult GetDrawing1()
        {
            var drawSquare = new DrawSquare(2, 2);
            
            return Ok(drawSquare);
        }

        // TODO: Il faut ajouter une nouvelle action pour dessiner la séquence mentionnée dans l'énoncé
        [HttpGet]
        public ActionResult Getdrawing2()
        {
            var circle = new DrawCircle(1, 1);

            var wait3 = new Wait(3);

            var square1 = new DrawSquare(0, 2);
            var square2 = new DrawSquare(2, 2);

            var wait1 = new Wait(1);

            var star = new DrawStar(1, 3, 20);

            circle.DrawingEvents = new List<DrawingEvent> { wait3 };
            wait3.DrawingEvents = new List<DrawingEvent> {new ChangeColor("red"), square1, square2, wait1 };
            wait1.DrawingEvents = new List<DrawingEvent> {new ChangeColor("yellow"), star };

            return Ok(circle);
        }
    
    }
}
