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


        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id,  CelestialObject celestialObject)
        {
            CelestialObject foundObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (foundObject != null)
            {
                foundObject.Name = celestialObject.Name;
                foundObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
                foundObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

                _context.CelestialObjects.Update(foundObject);
                _context.SaveChanges();

                return NoContent();
            }

            return NotFound();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            CelestialObject foundObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (foundObject != null)
            {
                foundObject.Name = name;

                _context.CelestialObjects.Update(foundObject);
                _context.SaveChanges();

                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<CelestialObject> foundObjects = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
            if (foundObjects != null && foundObjects.Any())
            {
                _context.CelestialObjects.RemoveRange(foundObjects);
                _context.SaveChanges();

                return NoContent();
            }

            return NotFound();
        }

    }
}
