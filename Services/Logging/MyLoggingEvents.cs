using Microsoft.Extensions.Logging;

public static class MyLogEvents
{
    public static EventId GenerateItems = new EventId(1000,"GenerateItems");
    public static EventId ListItems     = new EventId(1000,"ListItems");
    public static EventId GetItem       = new EventId(2000,"GetItem");
    public static EventId InsertItem    = new EventId(3000,"InsertItem");
    public static EventId UpdateItem    = new EventId(4000,"UpdateItem");
    public static EventId DeleteItem    = new EventId(5000,"DeleteItem");

    

}