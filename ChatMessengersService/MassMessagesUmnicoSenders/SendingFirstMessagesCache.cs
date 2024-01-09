using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessengersService.MassMessagesUmnicoSenders
{
    public sealed class SendingFirstMessagesCache
    {
        private Dictionary<int, CachedSendingFirstMessage> _cachedValues = new Dictionary<int, CachedSendingFirstMessage>();

        public void Add(int _Key, CachedSendingFirstMessage _CacheMessage, TimeSpan _Ttl)
        {
            _CacheMessage.TimeToLife = DateTimeOffset.UtcNow.Add(_Ttl);
            _cachedValues.Add(_Key, _CacheMessage);
        }

        public CachedSendingFirstMessage Get(int _Key)
        {
            ClearOld();

            if (!_cachedValues.ContainsKey(_Key))
                return null;

            return _cachedValues[_Key];
        }

        private void ClearOld()
        {
            var oldCache = _cachedValues
                .Where(pair => DateTimeOffset.UtcNow > pair.Value.TimeToLife)
                .ToList();
            if (oldCache.Count == 0)
                return;

            foreach (KeyValuePair<int, CachedSendingFirstMessage> pair in oldCache)
            {
                _cachedValues.Remove(pair.Key);
            }
        }
    }
}
