namespace CatalogApi.Common
{
    /// <summary>Roles soportados por el backoffice. Evita strings mágicos dispersos por la solución.</summary>
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Viewer = "Viewer";

        /// <summary>Lectura: Viewer o Admin.</summary>
        public const string AdminOrViewer = "Admin,Viewer";
    }
}