using Data;
using Data.Entities;
using Data.Interface;
using Data;
using Data.Entities;
using Data.Interface;
using System;
using System.Linq;

namespace Service
{
    public class ConversionService
    {
        private readonly CurrencyConverterContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public ConversionService(
            CurrencyConverterContext context,
            IUserRepository userRepository,
            ICurrencyRepository currencyRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _currencyRepository = currencyRepository;
        }

        public decimal ConvertCurrency(int userId, int fromCode, int toCode, decimal amount)
        {
            var user = _userRepository.GetUserById(userId) 
                       ?? throw new InvalidOperationException("Usuario no encontrado.");

            // obtener subscription del usuario
            var subscription = _context.Subscriptions.FirstOrDefault(s => s.UserId == userId && s.IsActive);

            if (subscription == null)
            {
                // Usuario sin suscripción activa -> denegar
                throw new InvalidOperationException("Usuario sin suscripción activa.");
            }

            // calcular consumos en últimos 30 días
            var since = DateTime.UtcNow.AddDays(-30);
            var conversionsLast30 = _context.ConversionHistories
                .Count(h => h.UserId == userId && h.Date >= since);

            int allowed = subscription.Type switch
            {
                Data.Enums.SubscriptionType.Free => 10,
                Data.Enums.SubscriptionType.Trial => 100,
                Data.Enums.SubscriptionType.Pro => int.MaxValue,
                _ => 0
            };

            if (conversionsLast30 >= allowed)
            {
                throw new InvalidOperationException("Límite de conversiones del mes alcanzado.");
            }

            var from = _currencyRepository.GetByCode(fromCode) 
                       ?? throw new InvalidOperationException("Moneda origen no encontrada.");
            var to = _currencyRepository.GetByCode(toCode) 
                     ?? throw new InvalidOperationException("Moneda destino no encontrada.");

            // Fórmula: pasar a USD con IC (1 unidad de moneda * IC = USD), luego a moneda destino
            // result = amount * from.IC / to.IC
            if (to.ConversionRate <= 0) throw new InvalidOperationException("Índice de convertibilidad inválido para moneda destino.");

            var result = amount * from.ConversionRate / to.ConversionRate;

            // guardar historial
            var history = new ConversionHistory
            {
                UserId = userId,
                FromCurrencyCode = fromCode,
                ToCurrencyCode = toCode,
                Amount = amount,
                Result = result,
                Date = DateTime.UtcNow
            };

            _context.ConversionHistories.Add(history);
            _context.SaveChanges();

            return result;
        }
    }
}
