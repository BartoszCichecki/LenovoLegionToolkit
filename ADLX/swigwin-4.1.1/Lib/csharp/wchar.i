/* -----------------------------------------------------------------------------
 * wchar.i
 *
 * Typemaps for the wchar_t type
 * wchar_t * is mapped to a C# Unicode string (UTF16) and is passed around by value.
 * wchar_t * support includes wchar_t as a 2 byte type (Windows) and a 4 byte type
 * (most Unix systems).
 *
 * Support code for wide strings can be turned off by defining SWIG_CSHARP_NO_WSTRING_HELPER
 * ----------------------------------------------------------------------------- */

#if !defined(SWIG_CSHARP_NO_WSTRING_HELPER)
#if !defined(SWIG_CSHARP_WSTRING_HELPER_)
#define SWIG_CSHARP_WSTRING_HELPER_

%fragment("<wchar.h>"); // TODO: %fragment("<wchar.h", "runtime");

%insert(runtime) %{
/* Callback for returning strings to C# without leaking memory */
typedef void * (SWIGSTDCALL* SWIG_CSharpWStringHelperCallback)(const wchar_t *, int length);
static SWIG_CSharpWStringHelperCallback SWIG_csharp_wstring_with_length_callback = NULL;
%}

%insert(header) %{
static void * SWIG_csharp_wstring_callback(const wchar_t *s) {
  return SWIG_csharp_wstring_with_length_callback(s, (int)wcslen(s));
}
%}


%pragma(csharp) imclasscode=%{
  protected class SWIGWStringHelper {

    [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]
    public delegate string SWIGWStringDelegate(global::System.IntPtr message, int length);
    static SWIGWStringDelegate wstringUTF16Delegate = new SWIGWStringDelegate(CreateWStringFromUTF16);
    static SWIGWStringDelegate wstringUTF32Delegate = new SWIGWStringDelegate(CreateWStringFromUTF32);

    [global::System.Runtime.InteropServices.DllImport("$dllimport", EntryPoint="SWIGRegisterWStringCallback_$module")]
    public static extern void SWIGRegisterWStringCallback_$module(SWIGWStringDelegate wstringUTF16Delegate, SWIGWStringDelegate wstringUTF32Delegate);

    public static string CreateWStringFromUTF16([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]global::System.IntPtr cString, int length) {
      return global::System.Runtime.InteropServices.Marshal.PtrToStringUni(cString, length);
    }

    public static string CreateWStringFromUTF32([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]global::System.IntPtr cString, int length) {
      if (length == 0)
        return string.Empty;

      byte[] buffer = new byte[length * 4];
      global::System.Runtime.InteropServices.Marshal.Copy(cString, buffer, 0, buffer.Length);
      return global::System.Text.Encoding.UTF32.GetString(buffer);
    }

    static SWIGWStringHelper() {
      SWIGRegisterWStringCallback_$module(wstringUTF16Delegate, wstringUTF32Delegate);
    }
  }

  static protected SWIGWStringHelper swigWStringHelper = new SWIGWStringHelper();
%}

%insert(runtime) %{
#ifdef __cplusplus
extern "C"
#endif
SWIGEXPORT void SWIGSTDCALL SWIGRegisterWStringCallback_$module(SWIG_CSharpWStringHelperCallback callback_utf16, SWIG_CSharpWStringHelperCallback callback_utf32) {
  SWIG_csharp_wstring_with_length_callback = sizeof(wchar_t) == 2 ? callback_utf16 : callback_utf32;
}
%}
#endif // SWIG_CSHARP_WSTRING_HELPER_
#endif // SWIG_CSHARP_NO_WSTRING_HELPER

#if !defined(SWIG_CSHARP_NO_WSTRING_EXCEPTION_HELPER)
#if !defined(SWIG_CSHARP_WSTRING_EXCEPTION_HELPER_)
#define SWIG_CSHARP_WSTRING_EXCEPTION_HELPER_

%insert(runtime) %{
/* Callback for returning strings to C# without leaking memory */
typedef void (SWIGSTDCALL* SWIG_CSharpWStringExceptionHelperCallback)(const wchar_t *, int length);
static SWIG_CSharpWStringExceptionHelperCallback SWIG_csharp_ApplicationException_callback = NULL;
%}

%pragma(csharp) imclasscode=%{
  protected class SWIGWStringExceptionHelper {

    [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]
    public delegate void SWIGWStringExceptionDelegate(global::System.IntPtr message, int length);
    static SWIGWStringExceptionDelegate applicationExceptionUTF16Delegate = new SWIGWStringExceptionDelegate(SetPendingApplicationExceptionUTF16);
    static SWIGWStringExceptionDelegate applicationExceptionUTF32Delegate = new SWIGWStringExceptionDelegate(SetPendingApplicationExceptionUTF32);

    [global::System.Runtime.InteropServices.DllImport("$dllimport", EntryPoint="SWIGRegisterWStringExceptionCallback_$module")]
    public static extern void SWIGRegisterWStringExceptionCallback_$module(SWIGWStringExceptionDelegate applicationExceptionUTF16Delegate, SWIGWStringExceptionDelegate applicationExceptionUTF32Delegate);

    static string CreateWStringFromUTF16([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]global::System.IntPtr cString, int length) {
      return global::System.Runtime.InteropServices.Marshal.PtrToStringUni(cString, length);
    }

    public static string CreateWStringFromUTF32([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]global::System.IntPtr cString, int length) {
      if (length == 0)
        return string.Empty;

      byte[] buffer = new byte[length * 4];
      return global::System.Text.Encoding.UTF32.GetString(buffer);
    }

    static void SetPendingApplicationExceptionUTF16([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]global::System.IntPtr cString, int length) {
      string message = SWIGWStringHelper.CreateWStringFromUTF16(cString, length);
      SWIGPendingException.Set(new global::System.ApplicationException(message, SWIGPendingException.Retrieve()));
    }

    static void SetPendingApplicationExceptionUTF32([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]global::System.IntPtr cString, int length) {
      string message = SWIGWStringHelper.CreateWStringFromUTF32(cString, length);
      SWIGPendingException.Set(new global::System.ApplicationException(message, SWIGPendingException.Retrieve()));
    }

    static SWIGWStringExceptionHelper() {
      SWIGRegisterWStringExceptionCallback_$module(applicationExceptionUTF16Delegate, applicationExceptionUTF32Delegate);
    }
  }

  static protected SWIGWStringExceptionHelper swigWStringExceptionHelper = new SWIGWStringExceptionHelper();
%}

%insert(runtime) %{
#ifdef __cplusplus
extern "C"
#endif
SWIGEXPORT void SWIGSTDCALL SWIGRegisterWStringExceptionCallback_$module(SWIG_CSharpWStringExceptionHelperCallback callback_utf16, SWIG_CSharpWStringExceptionHelperCallback callback_utf32) {
  SWIG_csharp_ApplicationException_callback = sizeof(wchar_t) == 2 ? callback_utf16 : callback_utf32;
}
%}

#endif // SWIG_CSHARP_WSTRING_EXCEPTION_HELPER_
#endif // SWIG_CSHARP_NO_WSTRING_EXCEPTION_HELPER


// wchar_t
%typemap(ctype) wchar_t "wchar_t"
%typemap(imtype) wchar_t "char" // Requires adding CharSet=CharSet.Unicode to the DllImport to correctly marshal Unicode characters
%typemap(cstype) wchar_t "char"

%typemap(csin) wchar_t "$csinput"
%typemap(csout, excode=SWIGEXCODE) wchar_t {
    char ret = $imcall;$excode
    return ret;
  }
%typemap(csvarin, excode=SWIGEXCODE2) wchar_t %{
    set {
      $imcall;$excode
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) wchar_t %{
    get {
      char ret = $imcall;$excode
      return ret;
    } %}

%typemap(in) wchar_t %{ $1 = ($1_ltype)$input; %}
%typemap(out) wchar_t %{ $result = (wchar_t)$1; %}

%typemap(typecheck) wchar_t = char;

// wchar_t *

%fragment("Swig_csharp_UTF16ToWCharPtr", "header") %{
/* For converting from .NET UTF16 (2 byte unicode) strings. wchar_t is 2 bytes on Windows, 4 bytes on Linux. */
static wchar_t * Swig_csharp_UTF16ToWCharPtr(const unsigned short *str) {
  if (sizeof(wchar_t) == 2) {
    return (wchar_t *)str;
  } else {
    wchar_t *result = 0;

    if (str) {
      const unsigned short *pBegin(str);
      const unsigned short *pEnd(pBegin);
      wchar_t *ptr = 0;

      while (*pEnd != 0)
        ++pEnd;

#ifdef __cplusplus
      result = ptr = new wchar_t[pEnd - pBegin + 1];
#else
      result = ptr = (wchar_t *)malloc(sizeof(wchar_t) * (pEnd - pBegin + 1));
#endif
      while(pBegin != pEnd)
        *ptr++ = *pBegin++;
      *ptr++ = 0;
    }

    return result;
  }
}
%}

%fragment("Swig_csharp_UTF16ToWCharPtrFree", "header") %{
static void Swig_csharp_UTF16ToWCharPtrFree(wchar_t *str) {
  if (sizeof(wchar_t) != 2) {
#ifdef __cplusplus
    delete [] str;
#else
    free(str);
#endif
  }
}
%}

%typemap(ctype, out="void *") wchar_t * "unsigned short *"
%typemap(imtype,
         inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]",
         outattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]",
         directorinattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]",
         directoroutattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]"
         ) wchar_t * "string"
%typemap(cstype) wchar_t * "string"
%typemap(csdirectorin) wchar_t * "$iminput"
%typemap(csdirectorout) wchar_t * "$cscall"

%typemap(csin) wchar_t * "$csinput"
%typemap(csout, excode=SWIGEXCODE) wchar_t * {
    string ret = $imcall;$excode
    return ret;
  }
%typemap(csvarin, excode=SWIGEXCODE2) wchar_t * %{
    set {
      $imcall;$excode
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) wchar_t * %{
    get {
      string ret = $imcall;$excode
      return ret;
    } %}

%typemap(in, fragment="Swig_csharp_UTF16ToWCharPtr") wchar_t *
%{ $1 = Swig_csharp_UTF16ToWCharPtr($input); %}

%typemap(out) wchar_t * %{ $result = $1 ? SWIG_csharp_wstring_callback($1) : 0; %}

%typemap(freearg, fragment="Swig_csharp_UTF16ToWCharPtrFree") wchar_t *
%{ Swig_csharp_UTF16ToWCharPtrFree($1); %}

%typemap(directorout, warning=SWIGWARN_TYPEMAP_DIRECTOROUT_PTR_MSG) wchar_t *
%{ $result = Swig_csharp_UTF16ToWCharPtr($input); %}

%typemap(directorin) wchar_t * %{ $input = SWIG_csharp_wstring_with_length_callback($1, (int)wcslen($1)); %}

%typemap(typecheck) wchar_t * = char *;

%typemap(throws, canthrow=1, fragment="<wchar.h>") wchar_t *
%{ SWIG_csharp_ApplicationException_callback($1, (int)wcslen($1));
   return $null; %}


/* Default typemap for handling wchar_t * members (based on char * in swig.swg) */

#ifdef __cplusplus
%typemap(memberin,fragment="<wchar.h>") wchar_t * {
  delete [] $1;
  if ($input && sizeof(wchar_t) == 2) {
    $1 = ($1_type) (new wchar_t[wcslen((const wchar_t *)$input)+1]);
    wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
    $1 = $input;
    $input = 0;
  }
}
%typemap(memberin,warning=SWIGWARN_TYPEMAP_WCHARLEAK_MSG,fragment="<wchar.h>") const wchar_t * {
  if ($input && sizeof(wchar_t) == 2) {
    $1 = ($1_type) (new wchar_t[wcslen((const wchar_t *)$input)+1]);
    wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
    $1 = $input;
    $input = 0;
  }
}
%typemap(globalin,fragment="<wchar.h>") wchar_t * {
  delete [] $1;
  if ($input && sizeof(wchar_t) == 2) {
    $1 = ($1_type) (new wchar_t[wcslen((const wchar_t *)$input)+1]);
    wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
    $1 = $input;
    $input = 0;
  }
}
%typemap(globalin,warning=SWIGWARN_TYPEMAP_WCHARLEAK_MSG,fragment="<wchar.h>") const wchar_t * {
  if ($input && sizeof(wchar_t) == 2) {
    $1 = ($1_type) (new wchar_t[wcslen((const wchar_t *)$input)+1]);
    wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
    $1 = $input;
    $input = 0;
  }
}
#else
%typemap(memberin,fragment="<wchar.h>") wchar_t * {
  free($1);
  if ($input && sizeof(wchar_t) == 2) {
    $1 = ($1_type) malloc(wcslen((const wchar_t *)$input)+1);
    wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
    $1 = $input;
    $input = 0;
  }
}
%typemap(memberin,warning=SWIGWARN_TYPEMAP_WCHARLEAK_MSG,fragment="<wchar.h>") const wchar_t * {
  if ($input && sizeof(wchar_t) == 2) {
    $1 = ($1_type) malloc(wcslen((const wchar_t *)$input)+1);
    wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
    $1 = $input;
    $input = 0;
  }
}
%typemap(globalin,fragment="<wchar.h>") wchar_t * {
  free($1);
  if ($input && sizeof(wchar_t) == 2) {
    $1 = ($1_type) malloc(wcslen((const wchar_t *)$input)+1);
    wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
    $1 = $input;
    $input = 0;
  }
}
%typemap(globalin,warning=SWIGWARN_TYPEMAP_WCHARLEAK_MSG,fragment="<wchar.h>") const wchar_t * {
  if ($input && sizeof(wchar_t) == 2) {
    $1 = ($1_type) malloc(wcslen((const wchar_t *)$input)+1);
    wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
    $1 = $input;
    $input = 0;
  }
}

#endif
