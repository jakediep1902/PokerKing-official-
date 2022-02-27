using System.Collections;
using UnityEngine;

public class Light : MonoBehaviour
{
    private SpriteRenderer _sRenderer;
    private WaitForSeconds _threeSecs = new WaitForSeconds(0.1f);

    private Color _spriteColour;
    private float _originalalpha;

    private void Start()
    {
        _sRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Waiter());

        _spriteColour = _sRenderer.color;
        _originalalpha = _spriteColour.a;
        //Debug.Log("hello");
    }
    IEnumerator Waiter()
    {
        while (true)
        {
            //float wait_time = Random.Range(0.2f,0.5f);

            float wait_time = 0.1f;
            yield return new WaitForSecondsRealtime(wait_time);

            _spriteColour.a = 0.3f;
            _sRenderer.color = _spriteColour;

            yield return _threeSecs;

            _spriteColour.a = _originalalpha;
            _sRenderer.color = _spriteColour;
        }
    }
}