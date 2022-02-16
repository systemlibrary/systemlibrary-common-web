namespace SystemLibrary.Common.Web.Extensions
{
    /// <summary>
    /// Application builder options
    /// 
    /// For instance: control wether or not you want to force http redirection to https (both server and client side)
    /// </summary>
    /// <example>
    /// Inside your 'Startup' class:
    /// <code>
    /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    /// {
    ///     var options = new ApplicationBuilderOptions();
    ///     options.UseHttpRedrectionAndHsts = false;
    ///     app.CommonAppBuilder(options);
    /// }
    /// </code>
    /// </example>
    public class ApplicationBuilderOptions
    {
        public bool UseHttpRedirectionAndHsts = true;
        public bool UseRouting = true;
        public bool UseAuthenticationAndAuthorization = true;
        public bool UseControllerAndRazorPages = true;
        public bool UseStaticFiles = true;
    }
}
