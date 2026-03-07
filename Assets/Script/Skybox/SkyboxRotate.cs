using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{
public float speed = 1f;
private float rot;

void Start()
{
rot = RenderSettings.skybox.GetFloat("_Rotation");
}

void Update()
{
if (RenderSettings.skybox == null) return;
rot += speed * Time.deltaTime;
rot %= 360;
RenderSettings.skybox.SetFloat("_Rotation", rot);
}
}