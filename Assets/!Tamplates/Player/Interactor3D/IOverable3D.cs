using System;

public interface IOverable3D
{
    bool IsEnable();

    event Action<IClickable3D> OnOver;
    event Action<IClickable3D> OnUnOver;

    void Over();
    void UnOver();
}