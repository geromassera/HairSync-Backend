namespace Application.Exceptions
{
    /// <summary>
    /// Excepción para errores de solicitud incorrecta (HTTP 400).
    /// Usar cuando la solicitud del cliente es inválida o malformada,
    /// y no es un error de validación específico.
    /// </summary>
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepción para errores de autenticación (HTTP 401).
    /// OJO: A menudo, el framework maneja esto automáticamente.
    /// Úsala si en tu lógica de negocio necesitas forzar un fallo de autenticación.
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepción para errores de autorización/permisos (HTTP 403).
    /// Se lanza cuando un usuario está autenticado pero no tiene permisos
    /// para realizar una acción o acceder a un recurso.
    /// </summary>
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepción para recursos no encontrados (HTTP 404).
    /// La más común. Se lanza cuando una consulta a la BBDD o a otro servicio
    /// no devuelve el recurso esperado.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepción para conflictos de estado (HTTP 409).
    /// Muy útil para casos como "el email ya está registrado" al intentar
    /// crear un nuevo usuario. Indica que la solicitud es válida,
    /// pero no puede completarse por el estado actual del recurso.
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepción para errores de validación (HTTP 400).
    /// Específicamente para fallos en las reglas de validación de datos
    /// (ej. campos requeridos, formato de email incorrecto).
    /// </summary>
    public class ValidException : Exception
    {
        public ValidException(string message) : base(message) { }
    }

    public class AlreadyReviewedException : Exception
    {
        public AlreadyReviewedException(string message) : base(message)
        {
        }
    }
}
