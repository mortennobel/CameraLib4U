using UnityEngine;
using System.Collections;

public static class Vector3Ext  {
	/// <summary>
	/// Converts a point from Spherical coordinates to Cartesian (using positive
    /// * Y as up)
	/// </summary>
    public static Vector3 SphericalToCartesian(Vector3 sphereCoords) {
		Vector3 res = Vector3.zero;
        res.y = sphereCoords.x * Mathf.Sin(sphereCoords.z);
        float a = sphereCoords.x * Mathf.Cos(sphereCoords.z);
        res.x = a * Mathf.Cos(sphereCoords.y);
        res.z = a * Mathf.Sin(sphereCoords.y);

        return res;
    }
	
	/// <summary>
	/// Converts a point from Cartesian coordinates (using positive Y as up) to
    /// Spherical and stores the results in the store var. (Radius, Azimuth,
    /// Polar)
	/// </summary>
    public static Vector3 cartesianToSpherical(Vector3 cartCoords) {
        Vector3 store;
		if (cartCoords.x == 0)
            cartCoords.x = Mathf.Epsilon;
        store.x = Mathf.Sqrt((cartCoords.x * cartCoords.x)
                        + (cartCoords.y * cartCoords.y)
                        + (cartCoords.z * cartCoords.z));
        store.y = Mathf.Atan(cartCoords.z / cartCoords.x);
        if (cartCoords.x < 0)
            store.y += Mathf.PI;
        store.z = Mathf.Asin(cartCoords.y / store.x);
        return store;
    }
}
