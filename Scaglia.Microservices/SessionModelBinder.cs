using System.Web.Mvc;

namespace Scaglia.Microservices
{
    public sealed class SessionModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // recuperar a sessão que em tese foi criada no ScagliaMicroservicesAuthenticationProvider
            ISession sessao = (ISession)controllerContext.HttpContext.Session[Constantes.SESSION];

            return sessao;
        }
    }
}
