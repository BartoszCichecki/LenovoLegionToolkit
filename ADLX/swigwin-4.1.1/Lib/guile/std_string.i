/* -----------------------------------------------------------------------------
 * std_string.i
 *
 * SWIG typemaps for std::string
 * ----------------------------------------------------------------------------- */

// ------------------------------------------------------------------------
// std::string is typemapped by value
// This can prevent exporting methods which return a string
// in order for the user to modify it.
// However, I think I'll wait until someone asks for it...
// ------------------------------------------------------------------------

%include <exception.i>

%{
#include <string>
%}

namespace std {

    %naturalvar string;

    class string;

    %typemap(typecheck) string = char *;
    %typemap(typecheck) const string & = char *;

    %typemap(in) string (char * tempptr) {
        if (scm_is_string($input)) {
            tempptr = SWIG_scm2str($input);
            $1.assign(tempptr);
            SWIG_free(tempptr);
        } else {
            SWIG_exception(SWIG_TypeError, "string expected");
        }
    }

    %typemap(in) const string & ($*1_ltype temp, char *tempptr) {
        if (scm_is_string($input)) {
            tempptr = SWIG_scm2str($input);
            temp.assign(tempptr);
            SWIG_free(tempptr);
            $1 = &temp;
        } else {
            SWIG_exception(SWIG_TypeError, "string expected");
        }
    }

    %typemap(in) string * (char *tempptr) {
        if (scm_is_string($input)) {
            tempptr = SWIG_scm2str($input);
            $1 = new $*1_ltype(tempptr);
            SWIG_free(tempptr);
        } else {
            SWIG_exception(SWIG_TypeError, "string expected");
        }
    }

    %typemap(out) string {
        $result = SWIG_str02scm($1.c_str());
    }

    %typemap(out) const string & {
        $result = SWIG_str02scm($1->c_str());
    }

    %typemap(out) string * {
        $result = SWIG_str02scm($1->c_str());
    }

    %typemap(varin) string {
        if (scm_is_string($input)) {
	    char *tempptr = SWIG_scm2str($input);
            $1.assign(tempptr);
            SWIG_free(tempptr);
        } else {
            SWIG_exception(SWIG_TypeError, "string expected");
        }
    }

    %typemap(varout) string {
        $result = SWIG_str02scm($1.c_str());
    }

    %typemap(throws) string {
      scm_throw(scm_from_locale_symbol((char *) "swig-exception"),
                scm_list_n(SWIG_str02scm($1.c_str()), SCM_UNDEFINED));
    }

    %typemap(throws) const string & {
      scm_throw(scm_from_locale_symbol((char *) "swig-exception"),
                scm_list_n(SWIG_str02scm($1.c_str()), SCM_UNDEFINED));
    }
}
