using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            CelestialObject foundObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (foundObject != null)
            {
                foundObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
                return Ok(foundObject);
            }

            return NotFound();
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            List<CelestialObject> foundObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (foundObjects != null && foundObjects.Any())
            {
                foreach (CelestialObject foundObject in foundObjects)
                {
                    foundObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == foundObject.Id).ToList();
                }

                return Ok(foundObjects);
            }

            return NotFound();
        }

        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            List<CelestialObject> celestialObjects = new List<CelestialObject>();
            foreach (CelestialObject cObject in _context.CelestialObjects)
            {
                CelestialObject objectToAdd = cObject;
                objectToAdd.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == objectToAdd.Id).ToList();
                celestialObjects.Add(objectToAdd);
            }
            
            return Ok(celestialObjects);
        }

    }
}
