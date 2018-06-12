using static TradeObjects.QuikDataObj;

namespace TradeObjects.Strategies
{
    public interface Istrategies
    {
        ToQuikCommand GetToQUIKCommand(object glas);
    }
}
