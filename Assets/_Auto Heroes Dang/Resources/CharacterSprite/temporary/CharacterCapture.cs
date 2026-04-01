using System.IO;
using UnityEngine;

public class CharacterCapture : MonoBehaviour
{
    [SerializeField] private Camera _captureCamera; // 캡처용 카메라
    [SerializeField] private RenderTexture _renderTexture; // 렌더 텍스처

    public void ExportToPNG(string fileName = "")
    {

        // 저장 경로 설정, 택1

        string folder = "_Auto Heroes Dang/Resources/CharacterSprite/ScreenShot";
        string folderCheck = Path.Combine(Application.dataPath, folder);

        /*
        string folder = Path.Combine(Application.persistentDataPath, "Screenshots");

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        */

        string date = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        string fileNameDate = $"{fileName}_{date}.png";
        string path = Path.Combine(folderCheck, fileNameDate);

        // -------------------------- 여기까지 파일 이름, 경로 설정

        _captureCamera.targetTexture = _renderTexture;
        _captureCamera.Render();

        RenderTexture.active = _renderTexture;

        Texture2D texture = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.RGBA32, false); // 마지막 bool = 리니어 on / off
        texture.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
        texture.Apply(); 


        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        Debug.Log($"저장 성공!!! 경로: {path}");

        RenderTexture.active = null;

        Destroy(texture);


        /* 에디터 새로고침용
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        */
    }

}