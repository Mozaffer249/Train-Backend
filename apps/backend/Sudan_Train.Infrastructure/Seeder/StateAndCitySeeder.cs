using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudan_Train.Infrastructure.Seeder
{
    public class StateAndCitySeeder
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<StateAndCitySeeder> _logger;

        public StateAndCitySeeder(ApplicationDBContext context, ILogger<StateAndCitySeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                var regionsCount = await _context.Regions.CountAsync();
                if (regionsCount > 0)
                {
                    _logger.LogInformation("Regions, states, and cities already exist. Skipping seeding.");
                    return;
                }

                _logger.LogInformation("Seeding Sudanese regions, states, and cities...");

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var regionsData = GetSudaneseRegionsStatesAndCities();
                    int totalStates = 0;
                    int totalCities = 0;

                    foreach (var regionData in regionsData)
                    {
                        // Create and save region
                        var region = new Region
                        {
                            NameEn = regionData.RegionNameEn,
                            NameAr = regionData.RegionNameAr,
                            Code = regionData.RegionCode
                        };

                        await _context.Regions.AddAsync(region);
                        await _context.SaveChangesAsync(); // Save to get the RegionId

                        // Create states for this region
                        foreach (var stateData in regionData.States)
                        {
                            var state = new State
                            {
                                NameEn = stateData.StateNameEn,
                                NameAr = stateData.StateNameAr,
                                RegionId = region.Id
                            };

                            await _context.States.AddAsync(state);
                            await _context.SaveChangesAsync(); // Save to get the StateId

                            // Create cities for this state
                            var cities = stateData.Cities.Select(cityData => new City
                            {
                                NameEn = cityData.CityNameEn,
                                NameAr = cityData.CityNameAr,
                                StateId = state.Id
                            }).ToList();

                            await _context.Cities.AddRangeAsync(cities);
                            totalStates++;
                            totalCities += cities.Count;
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"Successfully seeded {regionsData.Count} regions, {totalStates} states, and {totalCities} cities.");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding regions, states, and cities.");
                throw;
            }
        }

        private List<(string RegionNameEn, string RegionNameAr, string RegionCode,
            List<(string StateNameEn, string StateNameAr, List<(string CityNameEn, string CityNameAr)> Cities)> States)>
            GetSudaneseRegionsStatesAndCities()
        {
            return new List<(string, string, string, List<(string, string, List<(string, string)>)>)>
            {
                // 1. Khartoum Region
                ("Khartoum", "الخرطوم", "KRT", new List<(string, string, List<(string, string)>)>
                {
                    ("Khartoum", "الخرطوم", new List<(string, string)>
                    {
                        ("Khartoum", "الخرطوم"),
                        ("Omdurman", "أم درمان"),
                        ("Khartoum North", "الخرطوم بحري"),
                        ("Bahri", "بحري"),
                        ("Jabal Awliya", "جبل أولياء"),
                        ("Sharg an Nil", "شرق النيل"),
                        ("Karrari", "كرري"),
                        ("Umbadda", "أمبدة")
                    })
                }),

                // 2. Eastern Region
                ("Eastern", "الشرقية", "EST", new List<(string, string, List<(string, string)>)>
                {
                    ("Kassala", "كسلا", new List<(string, string)>
                    {
                        ("Kassala", "كسلا"),
                        ("Khashm el Girba", "خشم القربة"),
                        ("New Halfa", "حلفا الجديدة"),
                        ("Aroma", "أروما"),
                        ("Halfa", "حلفا"),
                        ("Wad al Hilaiw", "ود الحليو"),
                        ("Telkok", "تلكوك"),
                        ("Nahr Atbara", "نهر عطبرة")
                    }),
                    ("Red Sea", "البحر الأحمر", new List<(string, string)>
                    {
                        ("Port Sudan", "بورت سودان"),
                        ("Suakin", "سواكن"),
                        ("Tokar", "طوكر"),
                        ("Haya", "هيا"),
                        ("Sinkat", "سنكات"),
                        ("Agig", "عقيق"),
                        ("Gunob", "جونب"),
                        ("Durdeib", "دورديب")
                    }),
                    ("Gedaref", "القضارف", new List<(string, string)>
                    {
                        ("Gedaref", "القضارف"),
                        ("Doka", "الدوكة"),
                        ("Gallabat", "القلابات"),
                        ("Fau", "الفاو"),
                        ("Fashaga", "الفشقة"),
                        ("Al Faw", "الفاو"),
                        ("Al Qureisha", "القريشة"),
                        ("Basunda", "باسندة")
                    })
                }),

                // 3. Northern Region
                ("Northern", "الشمالية", "NTH", new List<(string, string, List<(string, string)>)>
                {
                    ("River Nile", "نهر النيل", new List<(string, string)>
                    {
                        ("Atbara", "عطبرة"),
                        ("Ed Damer", "الدامر"),
                        ("Berber", "بربر"),
                        ("Abu Hamad", "أبو حمد"),
                        ("Shendi", "شندي"),
                        ("Al Matammah", "المتمة"),
                        ("Kabushiya", "كبوشية"),
                        ("Al Buwayhah", "البويهة")
                    }),
                    ("Northern", "الشمالية", new List<(string, string)>
                    {
                        ("Dongola", "دنقلا"),
                        ("Karima", "كريمة"),
                        ("Merowe", "مروي"),
                        ("Delgo", "دلقو"),
                        ("Wadi Halfa", "وادي حلفا"),
                        ("Abri", "عبري"),
                        ("Al Golid", "الجوليد"),
                        ("Al Burgaig", "البرقيق")
                    })
                }),

                // 4. Central Region
                ("Central", "الوسطى", "CNT", new List<(string, string, List<(string, string)>)>
                {
                    ("Gezira", "الجزيرة", new List<(string, string)>
                    {
                        ("Wad Medani", "ود مدني"),
                        ("Al Managil", "المناقل"),
                        ("Hasaheisa", "الحصاحيصا"),
                        ("Rufaa", "رفاعة"),
                        ("Kamlin", "كامبلين"),
                        ("Um Al Qura", "أم القرى"),
                        ("24 Al Qarashi", "٢٤ القرشي")
                    }),
                    ("White Nile", "النيل الأبيض", new List<(string, string)>
                    {
                        ("Kosti", "كوستي"),
                        ("Rabak", "ربك"),
                        ("Ed Dueim", "الدويم"),
                        ("Um Rimta", "أم رمته"),
                        ("Tandalty", "تندلتي"),
                        ("Al Jabalain", "الجبلين"),
                        ("Al Geteina", "القطينة"),
                        ("Jebelein", "جبلين"),
                        ("Al Qitena", "القطينة"),
                    }),
                    ("Blue Nile", "النيل الأزرق", new List<(string, string)>
                    {
                        ("Ad Damazin", "الدمازين"),
                        ("Roseires", "الروصيرص"),
                        ("Kurmuk", "كرمك"),
                        ("Geissan", "قيسان"),
                        ("Baw", "باو"),
                        ("Wad Al Mahi", "ود الماحي"),
                        ("Al Tadamon", "التضامن")
                    }),
                    ("Sennar", "سنار", new List<(string, string)>
                    {
                        ("Sinja", "سنجة"),
                        ("Sennar", "سنار"),
                        ("Dinder", "الدندر"),
                        ("Abu Hujar", "أبو حجار"),
                        ("Al Suki", "السوكي"),
                        ("Al Dali", "الدالي"),
                        ("East Sennar", "شرق سنار")
                    })
                }),

                // 5. Kordofan Region
                ("Kordofan", "كردفان", "KRD", new List<(string, string, List<(string, string)>)>
                {
                    ("North Kordofan", "شمال كردفان", new List<(string, string)>
                    {
                        ("El Obeid", "الأبيض"),
                        ("Bara", "بارا"),
                        ("Sodiri", "سودري"),
                        ("Gebrat Al Sheikh", "جبرة الشيخ"),
                        ("Um Rawaba", "أم روابة"),
                        ("Al Rahad", "الرهد"),
                        ("En Nuhud", "النهود"),
                        ("Ghebeish", "غبيش"),
                        ("Umm Dam", "أم دم")
                    }),
                    ("South Kordofan", "جنوب كردفان", new List<(string, string)>
                    {
                        ("Kadugli", "كادقلي"),
                        ("Dilling", "الدلنج"),
                        ("Talodi", "تلودي"),
                        ("Rashad", "رشاد"),
                        ("Abu Kershola", "أبو كرشولا"),
                        ("Lagowa", "لقاوة"),
                        ("Abyei", "أبيي"),
                        ("Heiban", "هيبان")
                    }),
                    ("West Kordofan", "غرب كردفان", new List<(string, string)>
                    {
                        ("El Fula", "الفولة"),
                        ("Babanusa", "بابنوسة"),
                        ("Lagawa", "لقاوة"),
                        ("Abu Zabad", "أبو زبد"),
                        ("Al Salam", "السلام"),
                        ("Kailak", "كيلك"),
                        ("Ghubaish", "غبيش")
                    })
                }),

                // 6. Darfur Region
                ("Darfur", "دارفور", "DRF", new List<(string, string, List<(string, string)>)>
                {
                    ("North Darfur", "شمال دارفور", new List<(string, string)>
                    {
                        ("El Fasher", "الفاشر"),
                        ("Kutum", "كتم"),
                        ("Kabkabiya", "كبكبية"),
                        ("Mellit", "مليط"),
                        ("Tawila", "طويلة"),
                        ("Dar Es Salaam", "دار السلام"),
                        ("Um Kadadah", "أم كدادة"),
                        ("Saraf Omra", "سرف عمرة"),
                        ("Kornoi", "كرنوي")
                    }),
                    ("South Darfur", "جنوب دارفور", new List<(string, string)>
                    {
                        ("Nyala", "نيالا"),
                        ("Kas", "كاس"),
                        ("Tulus", "تلس"),
                        ("Rehaid Albirdi", "رهيد البردي"),
                        ("Buram", "برام"),
                        ("Adila", "عديلة"),
                        ("Kateila", "كتيلا"),
                        ("Sheiria", "شعيرية")
                    }),
                    ("East Darfur", "شرق دارفور", new List<(string, string)>
                    {
                        ("Ed Daein", "الضعين"),
                        ("Yassin", "ياسين"),
                        ("Abu Karinka", "أبو كارنكا"),
                        ("Adila", "عديلة"),
                        ("Assalaya", "عسلاية"),
                        ("Al Firdous", "الفردوس"),
                        ("Bahr Arab", "بحر العرب")
                    }),
                    ("West Darfur", "غرب دارفور", new List<(string, string)>
                    {
                        ("Geneina", "الجنينة"),
                        ("Kulbus", "كلبس"),
                        ("Beida", "بيضا"),
                        ("Sirba", "سربا"),
                        ("Habila", "حبيلة"),
                        ("Foro Baranga", "فور برنقا"),
                        ("Jebel Moon", "جبل مون"),
                        ("Krinding", "كرندنق")
                    }),
                    ("Central Darfur", "وسط دارفور", new List<(string, string)>
                    {
                        ("Zalingei", "زالنجي"),
                        ("Wadi Salih", "وادي صالح"),
                        ("Mukjar", "مكجر"),
                        ("Bindisi", "بندسي"),
                        ("Azum", "أزوم"),
                        ("Golo", "جولو"),
                        ("Rokero", "روكرو"),
                        ("Um Dukhun", "أم دخن")
                    })
                })
            };
        }
    }
}
