
[System.Serializable]
public class HandData
{
    public float time;
    public float distance;
    public float[] pos;
    public float[] quat;

    public HandData(float mytime, float mydistance, float[] mypos, float[] myquat)
    {
        time = mytime;
        distance = mydistance;
        pos = mypos;
        quat = myquat;
    }
}
