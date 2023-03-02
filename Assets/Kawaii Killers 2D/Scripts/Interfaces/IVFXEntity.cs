using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVFXEntity
{
    // Por ahora solo se requiere la posici�n, pero la idea es colocar aqu� todos los params
    //  requeridos por el manager VFXManager para crear los efectos visuales (puede ser color, material, etc)

    Vector3 Position { get; } 

}
