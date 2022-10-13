using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
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

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (celestialObject == null) return NotFound();

            celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name);

            if (!celestialObjects.Any()) return NotFound();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allCelestialObjects = _context.CelestialObjects.ToList();

            foreach (var celestialObject in allCelestialObjects)
            {
                celestialObject.Satellites = allCelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(allCelestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { celestialObject.Id });
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, CelestialObject toUpdate)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (celestialObject == null) return NotFound();

            //implement mapper
            celestialObject.Name = toUpdate.Name;
            celestialObject.OrbitalPeriod = toUpdate.OrbitalPeriod;
            celestialObject.OrbitedObjectId = toUpdate.OrbitedObjectId;

            _context.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id:int}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (celestialObject == null) return NotFound();

            celestialObject.Name = name;

            _context.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id);

            if (celestialObjects.Any()) return NotFound();

            _context.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
