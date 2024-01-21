using System.Collections;
using UnityEngine;

public class CharacterColorController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _chestMesh;

    [SerializeField] private Material _fireMaterial;
    [SerializeField] private Material _iceMaterial;
    [SerializeField] private Material _windMaterial;

    [SerializeField] private Material _auraMaterial;

    private Element? _currentElement = null;

    public void ChangeChestColor(Element element)
    {
        if (_currentElement == element)
            return;

        Material[] mats;

        switch (element)
        {
            case Element.Fire:
                mats = new Material[] { _fireMaterial, _auraMaterial };
                _chestMesh.materials = mats;
                _chestMesh.materials[1].SetColor("_Color", Color.red);
                StopAllCoroutines();
                StartCoroutine(ScaleDownAuraMaterial());
                _currentElement = element;
                break;
            case Element.Ice:
                mats = new Material[] { _iceMaterial, _auraMaterial };
                _chestMesh.materials = mats;
                _chestMesh.materials[1].SetColor("_Color", Color.blue);
                StopAllCoroutines();
                StartCoroutine(ScaleDownAuraMaterial());
                _currentElement = element;
                break;
            case Element.Wind:
                mats = new Material[] { _windMaterial, _auraMaterial };
                _chestMesh.materials = mats;
                _chestMesh.materials[1].SetColor("_Color", Color.green);
                StopAllCoroutines();
                StartCoroutine(ScaleDownAuraMaterial());
                _currentElement = element;
                break;
            default:
                break;
        }
    }

    private IEnumerator ScaleDownAuraMaterial()
    {
        _chestMesh.materials[1].SetFloat("_Scale", 1.1f);

        yield return new WaitForSeconds(Constants.PARRY_DURATION);

        _chestMesh.materials[1].SetFloat("_Scale", 0f);
    }
}
