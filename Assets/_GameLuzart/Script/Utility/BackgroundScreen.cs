using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScreen : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isSetCam2D = false;

    void Start()
    {
        // Lấy camera chính
        Camera mainCamera = Camera.main;

        // Tính toán góc nhìn của camera
        float halfFovRadians = mainCamera.fieldOfView * Mathf.Deg2Rad / 2f;
        // Tính khoảng cách từ camera đến background sprite
        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - spriteRenderer.transform.position.z);

        // Tính chiều cao của view frustum tại độ sâu của background sprite
        float visibleHeightAtDepth = /*2f * */distanceToCamera * Mathf.Tan(halfFovRadians);

        // Tính chiều rộng của view frustum tại độ sâu của background sprite
        float visibleWidthAtDepth = visibleHeightAtDepth * mainCamera.aspect;

        // Lấy kích thước của sprite trong đơn vị thế giới
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;

        // Tính toán tỷ lệ để background vừa với màn hình
        float heightScale = visibleHeightAtDepth / spriteHeight;
        float widthScale = visibleWidthAtDepth / spriteWidth;

        // Chọn tỷ lệ lớn hơn để đảm bảo sprite background đầy màn hình
        float scaleFactor = Mathf.Max(heightScale, widthScale);

        // Áp dụng tỷ lệ cho sprite background
        spriteRenderer.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }
    private void FitToCameraFOV()
    {
        // Góc mà camera có thể nhìn thấy phía trên trung tâm.
        float halfFovRadians = mainCamera.fieldOfView * Mathf.Deg2Rad / 2f;

        // Chiều cao từ trên xuống dưới của mặt cắt nhìn thấy,
        // tính bằng đơn vị không gian thế giới, tại độ sâu mục tiêu của chúng ta.
        float visibleHeightAtDepth = transform.localPosition.z * Mathf.Tan(halfFovRadians) * 2f;

        // Chiều rộng từ trên xuống dưới của mặt cắt nhìn thấy,
        // tính bằng đơn vị không gian thế giới, tại độ sâu mục tiêu của chúng ta.
        float visibleWidthAtDepth = visibleHeightAtDepth * Screen.width / Screen.height;

        // Bạn cũng có thể sử dụng Sprite.bounds cho việc này.
        float spriteHeight = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit;

        float spriteWidth = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit;


        // Tỷ lệ chiều cao chúng ta muốn lấp đầy lớn (hoặc nhỏ) bao nhiêu?
        Vector3 scaleFactor = new Vector3(visibleWidthAtDepth / spriteWidth, visibleHeightAtDepth / spriteHeight, 1);

        // Điều chỉnh tỷ lệ để phù hợp, đồng đều trên tất cả các trục.
        spriteRenderer.transform.localScale = scaleFactor;

    }
    private void FitWithCameraOthorgraphic()
    {
        // Lấy kích thước của Camera Orthographic
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Lấy kích thước của Sprite
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        // Tính toán tỷ lệ giữa kích thước của Camera và kích thước của Sprite
        float widthRatio = cameraWidth / spriteWidth;
        float heightRatio = cameraHeight / spriteHeight;

        // Chọn tỷ lệ lon nhat để đảm bảo Sprite phù hợp với Camera
        float scale = Mathf.Max(widthRatio, heightRatio);

        // Thay đổi kích thước của Sprite
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
