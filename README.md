MVCSugar
========

Collection of HtmlHelpers and other sweet things

This is an early release of some HTMLHelpers that we find ourselves using across multiple projects.

You can get the latest release from NuGet.

<pre class="nuget-button">Install-Package RedWall.MVCSugar</pre>

SessionHelper
====
`SessionHelper.Add(string key, object value)` 
Simple wrapper around storing the specified `value` with the provided `key` in the session object.

`SessionHelper.Get<T>(string key)` 
Retrieves the value from the session object with the specified `key` and casts the `object` to the type specified in the `T` generic parameter.

`SessionHelper.Get<T>(string key, Func<T> getIfEmpty)` 
Retrieves the value from the session object with the specified `key` and casts the `object` to the type specified in the `T` generic parameter. If there is no object in session with the specified `key` the `getIfEmpty` function will be executed and the return value will be stored in session and then returned to the caller.

ValidateReCaptchaAttribute
====
`[ValidateReCaptcha(RecaptchaSecret="<secret>")]`

Automatically validates a ReCaptcha when applied to an MVC Controller. Must provide the `RecapchaSecret` when applying the attribute.

Will add a `ReCaptcha` `ModelState` error if the validation fails.
