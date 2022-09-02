using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //异步加载 所以start用协程
    IEnumerator Start()
    {
        //加载预设
        AssetBundleCreateRequest request =
            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/prefabs/button.prefab.ab");
        yield return request;
        
        //加载图片
        AssetBundleCreateRequest request1 =
            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/res/title_bg_n.png.ab");
        yield return request1;

        AssetBundleRequest bundleRequest =
            request.assetBundle.LoadAssetAsync("Assets/BuildResources/UI/Prefabs/button.prefab");
        yield return bundleRequest;
        
        //实例化
        GameObject go = Instantiate(bundleRequest.asset) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
        
    }

    void Update()
    {
        
    }
}
