using System.Globalization;
using System.Net;
using System.Reflection;

namespace DrivingLicenseManagement.Infrastructure.Services.Email;

public sealed class EmailTemplateRenderer
{
    private const string TemplateResourcePrefix =
        "DrivingLicenseManagement.Infrastructure.Services.Email.Templates";

    public async Task<string> RenderAsync(string templateName, IReadOnlyDictionary<string, string> placeholders,
        CancellationToken cancellationToken = default)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{TemplateResourcePrefix}.{templateName}";

        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Email template '{templateName}' was not found.");
        using var reader = new StreamReader(stream);

        var template = await reader.ReadToEndAsync(cancellationToken);

        template = template.Replace(
            "{{Year}}",
            DateTime.UtcNow.Year.ToString(CultureInfo.InvariantCulture),
            StringComparison.Ordinal);

        foreach (var placeholder in placeholders)
        {
            template = template.Replace(
                $"{{{{{placeholder.Key}}}}}",
                WebUtility.HtmlEncode(placeholder.Value),
                StringComparison.Ordinal);
        }

        return template;
    }
}
