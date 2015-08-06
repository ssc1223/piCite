<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:word="http://schemas.microsoft.com/office/word/2003/wordml" xmlns:o="urn:schemas-microsoft-com:office:office">
  <xsl:output method="xml" encoding="utf-8" />
  <xsl:template match="/">
    <word:wordDocument xml:space="preserve">
      <word:body>
        <word:p>
          <xsl:apply-templates/>
        </word:p>
      </word:body>
    </word:wordDocument>
  </xsl:template>
  <xsl:template match="b">
    <word:r>
      <word:rPr>
        <word:b />
      </word:rPr>
      <word:t>
        <xsl:apply-templates/>
      </word:t>
    </word:r>
  </xsl:template>
  <xsl:template match="i">
    <word:r>
      <word:rPr>
        <word:i />
      </word:rPr>
      <word:t>
        <xsl:apply-templates/>
      </word:t>
    </word:r>
  </xsl:template>
  <xsl:template match="sup">
    <word:r>
      <word:rPr>
        <word:vertAlign word:val="superscript"/>
      </word:rPr>
      <word:t>
        <xsl:apply-templates/>
      </word:t>
    </word:r>
  </xsl:template>
  <xsl:template match="green">
    <word:r>
      <word:rPr>
        <word:color word:val="green" />
      </word:rPr>
      <word:t>
        <xsl:apply-templates/>
      </word:t>
    </word:r>
  </xsl:template>
  <xsl:template match="u">
    <word:r>
      <word:rPr>
        <word:u word:val="single"/>
      </word:rPr>
      <word:t>
        <xsl:apply-templates/>
      </word:t>
    </word:r>
  </xsl:template>
  <xsl:template match="br">
    <word:r>
      <word:br />
    </word:r>
  </xsl:template>
  <xsl:template match="span">
    <word:r>
      <word:t>
        <xsl:apply-templates/>
      </word:t>
    </word:r>
  </xsl:template>
  <xsl:template match="text()">
    <word:r>
      <word:t>
        <xsl:value-of select="."/>
      </word:t>
    </word:r>
  </xsl:template>
</xsl:stylesheet>