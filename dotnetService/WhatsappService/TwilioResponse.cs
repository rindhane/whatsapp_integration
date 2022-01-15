using System.Diagnostics;

namespace WhatsappService{
    public record subresourceUri (string media);
    public record statusResponse (
        string date_created,
        string date_updated,
        string date_sent,
        string account_sid,
        string to ,
        string from ,
        string messaging_service_sid,
        string body,
        string status,
        string num_segments,
        string num_media ,
        string direction,
        string api_version,
        string price,
        string price_unit,
        string error_code,
        string error_message,
        string uri,
        subresourceUri subresource_uris
        ) ;

}

