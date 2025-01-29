public class Resource
{
    private string _resName = "";
    public string resourceName {
        get{return _resName;}
        private set{_resName = value;}
    }
    public float Current {get;internal set;} = 0;
    public float Gain {get;internal set;} = 0;
    internal Resource(string resourceName){
        this._resName = resourceName;
    }
}
