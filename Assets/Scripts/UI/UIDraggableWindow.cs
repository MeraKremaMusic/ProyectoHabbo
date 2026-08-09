using UnityEngine;
using UnityEngine.EventSystems;

public sealed class UIDraggableWindow :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler
{
    private RectTransform ventana;
    private RectTransform limites;

    private Vector2 offsetPuntero;

    private const float MargenPantalla =
        18f;

    public void Configurar(
        RectTransform ventanaObjetivo,
        RectTransform limitesObjetivo)
    {
        ventana =
            ventanaObjetivo;

        limites =
            limitesObjetivo;
    }

    public void OnBeginDrag(
        PointerEventData eventData)
    {
        if (
            ventana == null ||
            limites == null
        )
        {
            return;
        }

        Camera camara =
            eventData.pressEventCamera;

        if (
            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    limites,
                    eventData.position,
                    camara,
                    out Vector2 puntoLocal
                )
        )
        {
            offsetPuntero =
                ventana.anchoredPosition -
                puntoLocal;
        }
    }

    public void OnDrag(
        PointerEventData eventData)
    {
        if (
            ventana == null ||
            limites == null
        )
        {
            return;
        }

        Camera camara =
            eventData.pressEventCamera;

        if (
            !RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    limites,
                    eventData.position,
                    camara,
                    out Vector2 puntoLocal
                )
        )
        {
            return;
        }

        Vector2 nuevaPosicion =
            puntoLocal +
            offsetPuntero;

        ventana.anchoredPosition =
            LimitarAPantalla(
                nuevaPosicion
            );
    }

    private Vector2 LimitarAPantalla(
        Vector2 posicion)
    {
        Rect rectLimites =
            limites.rect;

        Rect rectVentana =
            ventana.rect;

        float minimoX =
            rectLimites.xMin +
            rectVentana.width *
            ventana.pivot.x +
            MargenPantalla;

        float maximoX =
            rectLimites.xMax -
            rectVentana.width *
            (1f - ventana.pivot.x) -
            MargenPantalla;

        float minimoY =
            rectLimites.yMin +
            rectVentana.height *
            ventana.pivot.y +
            MargenPantalla;

        float maximoY =
            rectLimites.yMax -
            rectVentana.height *
            (1f - ventana.pivot.y) -
            MargenPantalla;

        if (minimoX > maximoX)
        {
            posicion.x =
                (minimoX + maximoX) *
                0.5f;
        }
        else
        {
            posicion.x =
                Mathf.Clamp(
                    posicion.x,
                    minimoX,
                    maximoX
                );
        }

        if (minimoY > maximoY)
        {
            posicion.y =
                (minimoY + maximoY) *
                0.5f;
        }
        else
        {
            posicion.y =
                Mathf.Clamp(
                    posicion.y,
                    minimoY,
                    maximoY
                );
        }

        return posicion;
    }
}
