using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SphericalSystems {
    public static Vector2 calcurateSphericalAngle(Quaternion quat, Vector3 dis) {//球面座標系における極角と方位角を求める。
        Vector3 anglePoint = quat * dis;//座標に対してどれくらい回転しているか、つまり回転を考慮した値を、Vector3で返す
        float radius = Mathf.Sqrt(anglePoint.x * anglePoint.x + anglePoint.y * anglePoint.y + anglePoint.z * anglePoint.z);
        float lengthXY = Mathf.Sqrt(anglePoint.x * anglePoint.x + anglePoint.y + anglePoint.y);//方位角計算のために、XY成分を求める
        float polarAngle = Mathf.Acos(anglePoint.y / radius);
        float azimuthAngle = Mathf.Atan(anglePoint.x / anglePoint.y);
        Vector2 sphericalAngle;
        sphericalAngle.x = polarAngle;
        sphericalAngle.y = azimuthAngle;
        return sphericalAngle;
    }

    public static Vector2 forwardSphericalAngle(Quaternion quat) {//Z軸のベクトルを極角と方位角で表現する
        return  calcurateSphericalAngle(quat, Vector3.forward);
    }
    public static Vector2 rightSphericalAngle(Quaternion quat) {//X軸のベクトルを極角と方位角で表現する。
        return  calcurateSphericalAngle(quat, Vector3.right);
    }
    public static Vector2 upSphericalAngle(Quaternion quat) {//Y軸のベクトルを極角と方位角で表現する。
        return  calcurateSphericalAngle(quat, Vector3.up);
    }

}
