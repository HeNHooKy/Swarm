using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swarm.Models;
using Swarm.Models.EFModel;
using Swarm.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Swarm.Controllers
{
    public class AppController : Controller
    {
        private readonly ILogger<AppController> _logger;
        private readonly Options.Meter _meterOptions;
        private readonly Context _context;

        public AppController(ILogger<AppController> logger, Context context, IOptions<Options.Meter> meterOptions)
        {
            _logger = logger;
            _meterOptions = meterOptions.Value;
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: build view with all meter replacement history for choosen flat (street, building, flatNumber).
        /// </summary>
        public IActionResult MeterReplacementHistory(string street, int? building, int? flatNumber)
        {
            var history = _context.MeterReplacementHistory
                .Where(h =>
                    street != null && h.Street.Equals(street) &&
                    building != null && h.Building == building.Value &&
                    flatNumber != null && h.FlatNumber == flatNumber.Value)
                .ToList();

            var streets = _context.Flats.Select(f => f.Street).Distinct();
            var buildings = _context.Flats.Select(f => f.Building).Distinct();
            var flatNumbers = _context.Flats.Select(f => f.FlatNumber).Distinct();

            TempData["streets"] = streets;
            TempData["buildings"] = buildings;
            TempData["flatNumbers"] = flatNumbers;

            return View(history);
        }

        /// <summary>
        /// GET: returns list of flat in the system.
        /// </summary>
        /// <param name="street">[optional filter]: flat street</param>
        /// <param name="building">[optional filter]: flat building</param>
        public IActionResult Flats(string street, int? building)
        {
            var flats = GetFlatsFromSetWith(_context.Flats, street, building).ToList();

            var meterRecords = MakeFlatCurrentRecordDictionary(flats);

            var streets = _context.Flats.Select(f => f.Street).Distinct();
            var buildings = _context.Flats.Select(f => f.Building).Distinct();

            var buzzyMettersNumb = _context.Flats
                .Include(f => f.Meter).Select(f => f.Meter);
            var freeMeters = _context.Meters.Except(buzzyMettersNumb).ToList();

            TempData["streets"] = streets;
            TempData["buildings"] = buildings;
            TempData["freeMeters"] = freeMeters;

            return View((flats, meterRecords));
        }

        /// <summary>
        /// POST: set new meter to the flat with parameters.
        /// </summary>
        /// <param name="street">Flat street</param>
        /// <param name="building">Flat building</param>
        /// <param name="flatNumber">Number of flat in the building.</param>
        /// <param name="meterFatoryNumber">Factory number of new meter for this flat.</param>
        [HttpPost]
        public IActionResult SetNewMeter(string street, int building, int flatNumber, int meterFatoryNumber)
        {
            var flat = _context.Flats
                .FirstOrDefault(f => 
                    f.Street.Equals(street) &&
                    f.Building == building &&
                    f.FlatNumber == flatNumber
                );

            if (flat == null)
            {
                TempData["message"] = $"The flat doesnt exist!";
                return RedirectToAction("Flats", "App");
            }

            var meter = _context.Meters
                .FirstOrDefault(m => m.FactoryNumber == meterFatoryNumber);

            if (meter == null)
            {
                TempData["message"] = $"The meter doesnt exist!";
                return RedirectToAction("Flats", "App");
            }

            var meterFlat = _context.Flats
                .FirstOrDefault(f => f.MeterFactoryNumber == meterFatoryNumber);

            if(meterFlat != null)
            {
                TempData["message"] = $"This meter already in use!";
                return RedirectToAction("Flats", "App");
            }
    
            flat.MeterFactoryNumber = meterFatoryNumber;
            _context.SaveChanges();

            return RedirectToAction("Flats", "App");
        }

        /// <summary>
        /// GET: Build page with all meters with filters.
        /// </summary>
        /// <param name="street">Filter: flat street</param>
        /// <param name="building">Filter: flat building</param>
        public IActionResult Meters(string street, int? building)
        {
            var allUnupdatedFlats = _context.Flats
                .Where(f => f.Meter.NextCheck <= DateTime.Now); //it should be injected

            var flats = GetFlatsFromSetWith(allUnupdatedFlats, street, building).ToList();

            var meterRecords = MakeFlatCurrentRecordDictionary(flats);

            var streets = allUnupdatedFlats.Select(f => f.Street).Distinct();
            var buildings = allUnupdatedFlats.Select(f => f.Building).Distinct();

            TempData["streets"] = streets;
            TempData["buildings"] = buildings;

            return View((flats, meterRecords));
        }

        /// <summary>
        /// POST: handles creating of new meter.
        /// </summary>
        /// <param name="factoryNumber">Meter factory number.</param>
        /// <param name="nextCheck">DateTime when meter should be updated.</param>
        [HttpPost]
        public IActionResult CreateMeter(int factoryNumber, DateTime nextCheck)
        {
            if (factoryNumber < 1)
            {
                TempData["message"] = $"Please, add correct meter factory number!";
                return RedirectToAction("Meters", "App");
            }

            if (nextCheck == DateTime.MinValue)
            {
                TempData["message"] = $"Please, add correct next check date!";
                return RedirectToAction("Meters", "App");
            }

            var meter = _context.Meters.FirstOrDefault(m => m.FactoryNumber == factoryNumber);

            if (meter != null)
            {
                TempData["message"] = $"Meter with this number already exists!";
                return RedirectToAction("Meters", "App");
            }

            var newMeter = new Meter
            {
                FactoryNumber = factoryNumber,
                NextCheck = nextCheck
            };

            

            _context.Meters.Add(newMeter);
            _context.SaveChanges();

            TempData["message"] = $"Meter created successfully!";
            return RedirectToAction("Meters", "App");
        }

        /// <summary>
        /// POST: Updates meters value to newValue.
        /// </summary>
        /// <param name="street">Flat street</param>
        /// <param name="building">Flat building</param>
        /// <param name="factoryNumber">Meter factory number</param>
        /// <param name="newValue">New meter value</param>
        [HttpPost]
        public IActionResult Meters(string street, int? building, int factoryNumber, int newValue)
        {
            var meter = _context.Meters
                .FirstOrDefault(m => m.FactoryNumber == factoryNumber);

            if(meter == null)
            {
                TempData["message"] = $"Meter with this number does not exist!";
                return RedirectToAction("Meters", "App", new { street, building });
            }

            if(meter.NextCheck > DateTime.Now)
            {
                TempData["message"] = $"Meter cannot be updated untill {meter.NextCheck}!";
                return RedirectToAction("Meters", "App", new { street, building });
            }
            var lastCheckRecord = _context.MeterRecords
                .FirstOrDefault(mr => mr.CheckDate == meter.LastCheck);

            if(lastCheckRecord != null && lastCheckRecord.MeterValue > newValue)
            {
                TempData["message"] = "Meter values cannot be decreased!";
                return RedirectToAction("Meters", "App", new { street, building });
            }

            var newRecord = new MeterRecords
            {
                MeterFactoryNumber = factoryNumber,
                MeterValue = newValue,
                CheckDate = DateTime.Now //it should be injected
            };

            meter.NextCheck = meter.NextCheck.AddDays(_meterOptions.ChecksFrequencyInDays);
            meter.LastCheck = newRecord.CheckDate;
            
            _context.MeterRecords.Add(newRecord);
            _context.SaveChanges();

            return RedirectToAction("Meters", "App", new { street, building });
        }

        /// <summary>
        /// Returns list of flats with spotted street and building.
        /// </summary>
        /// <param name="street">Building street</param>
        /// <param name="building">Flat Building</param>
        /// <returns>List of flats filtered by street and/or building.</returns>
        private IEnumerable<Flat> GetFlatsFromSetWith(IQueryable<Flat> flats, string street, int? building)
        {
            return flats
                .Include(f => f.Meter)
                .ThenInclude(m => m.MeterRecords)
                .Where(f =>
                    (street == null || f.Street.Equals(street)) &&
                    (building == null || f.Building == building.Value)
                );
        }

        /// <summary>
        /// It makes dictionary with flat - meter records pairs.
        /// </summary>
        private Dictionary<Flat, MeterRecords> MakeFlatCurrentRecordDictionary(IEnumerable<Flat> flats)
        {
            return flats.ToDictionary(
                f => f,
                f => f.Meter?.MeterRecords
                    .FirstOrDefault(mr => mr.CheckDate == f.Meter.LastCheck)
            );
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
