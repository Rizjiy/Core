<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:tns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
    <xsl:output method="xml" version="1.0" encoding="utf-8" indent="yes" />
    <xsl:strip-space elements="*" />
    <xsl:template match="*">
        <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
            <metadata>
                <xsl:for-each select="/tns:package/tns:metadata/*">
                    <xsl:choose>
                        <xsl:when test="local-name() = 'version'">
                            <xsl:element name="version">
                                <xsl:call-template name="nextVersion">
                                     <xsl:with-param name="text" select="text()" />
								</xsl:call-template>
                            </xsl:element>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:copy-of select="." />
                        </xsl:otherwise>
                    </xsl:choose>
                </xsl:for-each>
            </metadata>
            <xsl:copy-of select="/tns:package/tns:files" />
        </package>
    </xsl:template>
    <xsl:template name="nextVersion">
        <xsl:param name="text" />
        <xsl:param name="separator" select="'.'" />
        <xsl:choose>
            <xsl:when test="not(contains($text, $separator))">
                <xsl:value-of select="normalize-space($text) + 1" />
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="normalize-space(substring-before($text, $separator))" />.<xsl:call-template name="nextVersion">
                    <xsl:with-param name="text" select="substring-after($text, $separator)" />
                </xsl:call-template>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
</xsl:stylesheet>
