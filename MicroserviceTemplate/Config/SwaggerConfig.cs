using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceTemplate.Config
{
    /// <summary>
    /// Настройки swagger
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Отключить swagger, например для продуктивного окружения
        /// </summary>
        public bool IsEnabled { get; set; } = false;
        /// <summary>
        /// Префикс необходим для использования swagger в rancher под балансировкой ingress контроллера.
        /// Префикс применяется для указания пути к swagger.json файлу.
        /// </summary>
        public string EndpointPrefix { get; set; } = "";
    }
}
